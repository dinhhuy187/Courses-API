using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class CourseService(AppDbContext context) : ICourseService
{
    private readonly AppDbContext _context = context;
    public async Task<PagedResult<CourseListItemDto>> GetCoursesAsync(CourseQueryParameters q)
    {
        var query = _context.Courses.AsQueryable();
        if (!q.IncludeUnapproved)
            query = query.Where(c => c.IsApproved);
        
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
                CategoryId = c.CategoryId
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

    public async Task<CourseDetailDto?> GetByIdAsync(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null) return null;
        return new CourseDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            Level = course.Level,
            TeacherName = course.TeacherName,
            ImageUrl = course.ImageUrl,
            DurationHours = course.DurationHours,
            AverageRating = course.AverageRating,
            TotalPurchased = course.TotalPurchased,
            SellerId = course.SellerId,
            CategoryId = course.CategoryId,
            IsApproved = course.IsApproved,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }

    public async Task<CourseDetailDto> CreateAsync(CreateCourseDto dto)
    {
        var entity = new Course
        {   
            Title = dto.Title,
            Description = dto.Description ?? "",
            Price = dto.Price,
            Level = dto.Level,
            TeacherName = dto.TeacherName,
            ImageUrl = dto.ImageUrl,
            DurationHours = dto.DurationHours,
            CategoryId = dto.CategoryId,
            SellerId = dto.SellerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsApproved = false
        };
        _context.Courses.Add(entity);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id) ?? throw new InvalidOperationException("Created but cannot retrieve");
    }

    public async Task<CourseDetailDto?> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var entity = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (entity == null) return null;
        entity.Title = dto.Title ?? entity.Title;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price ?? entity.Price;
        entity.Level = dto.Level ?? entity.Level;
        entity.TeacherName = dto.TeacherName ?? entity.TeacherName;
        entity.ImageUrl = dto.ImageUrl;
        entity.DurationHours = dto.DurationHours ?? entity.DurationHours;
        entity.CategoryId = dto.CategoryId  ?? entity.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}