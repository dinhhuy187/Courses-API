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
        
        var query = context.Histories.AsQueryable();
        
        query = query.Where(h => h.UserId == userId);
        query = query.Include(h => h.Course);
        
        query = query.Where(h => h.Course!.IsApproved);
        query = query.Where(h => !h.Course!.IsRestricted);
        
        if (q.CategoryId.HasValue)
            query = query.Where(h => h.Course!.CategoryId == q.CategoryId);

        if (q.SellerId.HasValue)
            query = query.Where(h => h.Course!.SellerId == q.SellerId);

        if (!string.IsNullOrWhiteSpace(q.Level))
            query = query.Where(h => h.Course!.Level == q.Level);

        if (q.MinPrice.HasValue)
            query = query.Where(h => h.Course!.Price >= q.MinPrice.Value);

        if (q.MaxPrice.HasValue)
            query = query.Where(h => h.Course!.Price <= q.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(q.Q))
        {
            var text = q.Q.Trim();
            query = query.Where(h =>
                h.Course!.Title.Contains(text) ||
                h.Course!.Description.Contains(text));
        }

        query = query.OrderByDescending(h => h.CreatedAt);

        query = query.Include(h => h.Course!.Category);

        var total = await query.LongCountAsync();
        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(h => new CourseListItemDto
            {
                Id = h.Course!.Id,
                Title = h.Course!.Title,
                Price = h.Course!.Price,
                Level = h.Course!.Level,
                ImageUrl = h.Course!.ImageUrl,
                AverageRating = h.Course!.AverageRating,
                TotalPurchased = h.Course!.TotalPurchased,
                SellerId = h.Course!.SellerId,
                TeacherName = h.Course!.TeacherName,
                Description = h.Course!.Description,
                DurationHours = h.Course!.DurationHours,
                CategoryName = h.Course!.Category!.Name,
                IsApproved = h.Course!.IsApproved,
                IsRestricted = h.Course!.IsRestricted
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
            existingHistory.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            var course = await context.Courses.FindAsync(courseId);
            if (course == null || course.IsRestricted || !course.IsApproved)
                return false;

            var history = new History
            {
                UserId = userId,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };
            context.Histories.Add(history);
        }

        return await context.SaveChangesAsync() > 0;
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