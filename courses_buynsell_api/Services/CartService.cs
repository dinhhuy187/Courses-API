using System.Data;
using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.Cart;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class CartService(AppDbContext context) : ICartService
{
    public async Task<CartDto?> GetCartAsync(int userId)
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return null;
        return MapCart(cart);
    }

    public async Task<CartDto> AddItemAsync(AddCartItemDto dto)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == dto.CourseId);
        if (!courseExists) throw new KeyNotFoundException("Course not found");
        
        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == dto.UserId);

        if (cart == null)
        {
            cart = new Cart { UserId = (int)dto.UserId!, CreatedAt = DateTime.UtcNow };
            context.Carts.Add(cart);
        }
        
        var existing = cart.CartItems.FirstOrDefault(c => c.CourseId == dto.CourseId);
        if (existing != null)
        {
            throw new DuplicateNameException("Course already exists");
        }
        cart.CartItems.Add(new CartItem{ CourseId = dto.CourseId});
        
        await context.SaveChangesAsync();
        var reloaded = await context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == cart.Id);
        return MapCart(reloaded!);
    }

    public async Task<bool> RemoveItemAsync(int userId, int itemId)
    {
        var cart = await context.Carts.Include(c =>  c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return false;
        
        var cartItem = cart.CartItems.FirstOrDefault(i => i.Id == itemId);
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