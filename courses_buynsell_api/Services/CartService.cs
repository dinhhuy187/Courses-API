using System.Data;
using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.Cart;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Exceptions;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class CartService(AppDbContext context) : ICartService
{
    public async Task<IEnumerable<CourseListItemDto>> GetCartAsync(int userId)
    {
        
        var cart = await context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Course)
            .ThenInclude(c => c!.Category)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return new List<CourseListItemDto>();

        var response = cart.CartItems.Select(c => new CourseListItemDto
            {
                Id = c.Course!.Id,
                Title = c.Course!.Title,
                Price = c.Course!.Price,
                Level = c.Course!.Level,
                ImageUrl = c.Course!.ImageUrl,
                AverageRating = c.Course!.AverageRating,
                TotalPurchased = c.Course!.TotalPurchased,
                SellerId = c.Course!.SellerId,
                TeacherName = c.Course!.TeacherName,
                Description = c.Course!.Description,
                DurationHours = c.Course!.DurationHours,
                CategoryName = c.Course!.Category!.Name,
                IsApproved = c.Course!.IsApproved,
                IsRestricted = c.Course!.IsRestricted
            })
            .ToList();

        return response;
    }

    public async Task<CartDto> AddItemAsync(int userId, int courseId)
    {
        var course = await context.Courses.FindAsync(courseId);
        if (course == null) throw new NotFoundException("Course not found");
        
        if (course.IsRestricted || !course.IsApproved)
            throw new BadRequestException("This course can not be added");
        
        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
            context.Carts.Add(cart);
        }
        
        var existing = cart.CartItems.FirstOrDefault(c => c.CourseId == courseId);
        if (existing != null)
        {
            throw new BadRequestException("Course already exists");
        }
        cart.CartItems.Add(new CartItem{ CourseId = courseId});
        
        await context.SaveChangesAsync();
        var reloaded = await context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == cart.Id);
        return MapCart(reloaded!);
    }

    public async Task<bool> RemoveItemAsync(int userId, int itemId)
    {
        var cart = await context.Carts.Include(c =>  c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return false; 
        
        var cartItem = cart.CartItems.FirstOrDefault(i => i.CourseId == itemId);
        if (cartItem == null) return false;
        
        cart.CartItems.Remove(cartItem);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return false;
        context.CartItems.RemoveRange(cart.CartItems);
        await context.SaveChangesAsync();
        return true;
    }
    
    private static CartDto MapCart(Cart cart)
    {
        return new CartDto
        {
            Id  = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(i => new CartItemDto
            {
                Id = i.Id,
                CourseId = i.CourseId
            }).ToList()
        };
    }
}