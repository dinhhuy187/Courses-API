using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Exceptions;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class CourseService(AppDbContext context, IImageService imageService) : ICourseService
{
    public async Task<PagedResult<CourseListItemDto>> GetCoursesAsync(CourseQueryParameters q)
    {
        var query = context.Courses.AsQueryable();
        if (!(q.IncludeUnapproved ?? false))
            query = query.Where(c => c.IsApproved);
        
        if (!(q.IncludeRestricted ?? false))
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

        query = query
            .Include(c => c.Category)
            .Include(c => c.CourseContents)
            .Include(c => c.CourseSkills)
            .Include(c => c.TargetLearners);

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
                IsRestricted = c.IsRestricted,
                CommentCount = c.Enrollments.Count,
                CourseContents = c.CourseContents.Select(c => new CourseContentDto{ Id = c.Id, Title = c.Title, Description = c.Description ?? ""}).ToList(),
                CourseSkills = c.CourseSkills.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Name}).ToList(),
                TargetLearners = c.TargetLearners.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Description}).ToList()
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

    public async Task<CourseDetailDto?> GetByIdAsync(int id, bool isBuyer)
    {
        var course = await context.Courses
            .AsNoTracking()
            .Include(c => c.CourseContents)
            .Include(c => c.CourseSkills)
            .Include(c => c.TargetLearners)
            .Include(c => c.Category)
            .Include(c => c.Seller)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        if (isBuyer && (!course.IsApproved || course.IsRestricted)) return null;
        
        var commentCount = course.Reviews.Count;
        
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
            CategoryName = course.Category!.Name,
            IsApproved = course.IsApproved,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            Email = course.Seller!.Email,
            Phone = course.Seller.PhoneNumber!,
            CommentCount = commentCount,
            IsRestricted = course.IsRestricted,
            CourseContents = course.CourseContents.Select(c => new CourseContentDto{ Id = c.Id, Title = c.Title, Description = c.Description}).ToList(),
            CourseSkills = course.CourseSkills.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Name}).ToList(),
            TargetLearners = course.TargetLearners.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Description}).ToList()
        };
    }

    public async Task<IEnumerable<CourseStudentDto>> GetCourseStudents(int courseId, int sellerId, bool isAdmin)
    {
        var entity = await context.Courses
            .AsNoTracking()
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Buyer)
            .FirstOrDefaultAsync(c => c.Id == courseId);
        if (entity == null)
            throw new NotFoundException("Course not found");
        if (entity.SellerId != sellerId && !isAdmin)
            throw new UnauthorizedException("You do not have permission to view this course students");
        var result = entity.Enrollments.Select(e => new CourseStudentDto
            {
                StudentName = e.Buyer!.FullName,
                PurchasedAmount = entity.Price,
                EnrollAt = e.EnrollAt
            })
            .ToList();
        return result;
    }

    public async Task<CourseDetailDto> CreateAsync(CreateCourseDto dto, int userId)
    {
        var entity = new Course
        {
            Title = dto.Title,
            Description = dto.Description ?? "",
            Price = dto.Price,
            Level = dto.Level,
            TeacherName = dto.TeacherName,
            DurationHours = dto.DurationHours,
            CategoryId = dto.CategoryId,
            SellerId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsApproved = false,
            IsRestricted = false
        };

        if (dto.CourseContents != null)
        {
            foreach (var c in dto.CourseContents)
            {
                entity.CourseContents.Add(new CourseContent { Title = c.Title, Description = c.Description });
            }
        }

        if (dto.CourseSkills != null)
        {
            foreach (var c in dto.CourseSkills)
            {
                entity.CourseSkills.Add(new CourseSkill { Name = c.Description });
            }
        }

        if (dto.TargetLearners != null)
        {
            foreach (var c in dto.TargetLearners)
            {
                entity.TargetLearners.Add(new TargetLearner { Description = c.Description });
            }
        }

        if (dto.Image != null)
        {
            entity.ImageUrl = await imageService.UploadImageAsync(dto.Image);
        }

        context.Courses.Add(entity);
        await context.SaveChangesAsync();
        
        return await GetByIdAsync(entity.Id, false) ?? throw new InvalidOperationException("Created but cannot retrieve");
    }

    public async Task<CourseDetailDto?> UpdateAsync(int id, UpdateCourseDto dto, int sellerId)
    {
        var entity = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null) return null;

        if (entity.SellerId != sellerId)
            throw new UnauthorizedException("Update your course only!");

        entity.Title = dto.Title ?? entity.Title;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price ?? entity.Price;
        entity.Level = dto.Level ?? entity.Level;
        entity.TeacherName = dto.TeacherName ?? entity.TeacherName;
        entity.DurationHours = dto.DurationHours ?? entity.DurationHours;
        entity.CategoryId = dto.CategoryId ?? entity.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        if (dto.Image != null)
        {
            if (!string.IsNullOrEmpty(entity.ImageUrl))
                await imageService.DeleteImageAsync(entity.ImageUrl);

            entity.ImageUrl = await imageService.UploadImageAsync(dto.Image);
        }

        await context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id, false);
    }

    public async Task ApproveCourse(int courseId)
    {
        var course = await context.Courses.FindAsync(courseId);
        if (course == null) throw new NotFoundException("Course not found");
        if (course.IsApproved || course.IsRestricted)
            throw new InvalidOperationException("Course already approved or cannot approve restricted course");
        course.IsApproved = true;
        if (await context.SaveChangesAsync() < 1) throw new InvalidOperationException("Save changes failed");
    }
    
    public async Task<string> RestrictCourse(int courseId)
    {
        var course = await context.Courses.FindAsync(courseId);
        if (course == null) throw new NotFoundException("Course not found");
        if (course is { IsRestricted: false, IsApproved: false })
            throw new InvalidOperationException("Can't restrict unapproved course");
        if (course.IsRestricted)
        {
            course.IsRestricted = false;
            if (await context.SaveChangesAsync() < 1) throw new InvalidOperationException("Save changes failed");
            return "Course Unrestricted";
        }
        course.IsRestricted = true;
        if (await context.SaveChangesAsync() < 1) throw new InvalidOperationException("Save changes failed");
        return "Course Restricted";
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        context.Courses.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteImageAsync(int courseId, int userId)
    {
        var entity = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
        if (entity == null || entity.SellerId != userId)
            return false;
        if (string.IsNullOrEmpty(entity.ImageUrl))
            return true;
        await imageService.DeleteImageAsync(entity.ImageUrl);
        entity.ImageUrl = null;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<CourseContentDto> AddCourseContentAsync(int courseId, CourseContentDto dto)
    {
        var course = await context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var content = new CourseContent { Title = dto.Title, Description = dto.Description, CourseId = courseId };
        context.CourseContents.Add(content);
        await context.SaveChangesAsync();
        dto.Id = content.Id;
        return dto;
    }

    public async Task<bool> RemoveCourseContentAsync(int courseId, int contentId)
    {
        var c = await context.CourseContents.FirstOrDefaultAsync(x => x.Id == contentId && x.CourseId == courseId);
        if (c == null) return false;
        context.CourseContents.Remove(c);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<SkillTargetDto> AddCourseSkillAsync(int courseId, SkillTargetDto dto)
    {
        var course = await context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var skill = new CourseSkill { Name = dto.Description, CourseId = courseId };
        context.CourseSkills.Add(skill);
        await context.SaveChangesAsync();
        dto.Id = skill.Id;
        return dto;
    }

    public async Task<bool> RemoveCourseSkillAsync(int courseId, int skillId)
    {
        var c = await context.CourseSkills.FirstOrDefaultAsync(x => x.CourseId == courseId && x.Id == skillId);
        if (c == null) return false;
        context.CourseSkills.Remove(c);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<SkillTargetDto> AddTargetLearnerAsync(int courseId, SkillTargetDto dto)
    {
        var course = await context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var target = new TargetLearner { Description = dto.Description, CourseId = courseId };
        context.TargetLearners.Add(target);
        await context.SaveChangesAsync();
        dto.Id = target.Id;
        return dto;
    }

    public async Task<bool> RemoveTargetLearnerAsync(int courseId, int learnerId)
    {
        var t = await context.TargetLearners.FirstOrDefaultAsync(x => x.CourseId == courseId && x.Id == learnerId);
        if (t == null) return false;
        context.TargetLearners.Remove(t);
        await context.SaveChangesAsync();
        return true;
    }
}