using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Exceptions;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class HistoryService(AppDbContext context) : IHistoryService
{
    public async Task<PagedResult<CourseListItemDto>> GetMyHistory(CourseQueryParameters q, int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");
        
        var query = context.Courses.AsQueryable();
        
        query = query.Where(c => c.Histories.Any(h => h.UserId == userId));
        query = query.Where(c => c.IsApproved);
        query = query.Where(c => !c.IsRestricted);
        
        if (q.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == q.CategoryId);

        if (q.SellerId.HasValue)
            query = query.Where(c => c.SellerId == q.SellerId);

        if (!string.IsNullOrWhiteSpace(q.Level))
            query = query.Where(c => c.Level == q.Level);

        if (q.MinPrice.HasValue)
            query = query.Where(c => c.Price >= q.MinPrice.Value);

        if (q.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= q.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(q.Q))
        {
            var text = q.Q.Trim();
            query = query.Where(c =>
                c.Title.Contains(text) ||
                c.Description.Contains(text));
        }

        query = q.SortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "rating_desc" => query.OrderByDescending(c => c.AverageRating),
            "popular" => query.OrderByDescending(c => c.TotalPurchased),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        query = query.Include(c => c.Category);

        var total = await query.LongCountAsync();
        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(c => new CourseListItemDto
            {
                Id = c.Id,
                Title = c.Title,
                Price = c.Price,
                Level = c.Level,
                ImageUrl = c.ImageUrl,
                AverageRating = c.AverageRating,
                TotalPurchased = c.TotalPurchased,
                SellerId = c.SellerId,
                TeacherName = c.TeacherName,
                Description = c.Description,
                DurationHours = c.DurationHours,
                CategoryName = c.Category!.Name,
                IsApproved = c.IsApproved,
                IsRestricted = c.IsRestricted
            })
            .ToListAsync();
        return new PagedResult<CourseListItemDto>
        {
            Page = q.Page,
            PageSize = q.PageSize,
            TotalCount = total,
            Items = items,
        };
    }

    public async Task<bool> AddHistoryAsync(int userId, int courseId)
    {
        var existingHistory = await context.Histories
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CourseId == courseId);
        if (existingHistory != null)
        {
            return false;
        }
        
        var course = await context.Courses.FindAsync(courseId);
        if (course == null || course.IsRestricted || !course.IsApproved) 
            return false;
        
        var history = new History
        {
            UserId = userId,
            CourseId = courseId
        };
        context.Histories.Add(history);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearHistoriesAsync(int userId)
    {
        var histories = await context.Histories.Where(h => h.UserId == userId).ToListAsync();
        if (histories.Count == 0)
            return false;
        context.Histories.RemoveRange(histories);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveHistoryAsync(int userId, int courseId)
    {
        var history = await context.Histories.FirstOrDefaultAsync(h => h.UserId == userId && h.CourseId == courseId);
        if (history == null)
            return false;
        context.Histories.Remove(history);
        await context.SaveChangesAsync();
        return true;
    }
}