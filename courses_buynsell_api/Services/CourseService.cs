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
                CategoryName = c.Category!.Name
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
        var course = await context.Courses
            .AsNoTracking()
            .Include(c => c.CourseContents)
            .Include(c => c.CourseSkills)
            .Include(c => c.TargetLearners)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == id);
        
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
            CategoryName = course.Category!.Name,
            IsApproved = course.IsApproved,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            CourseContents = course.CourseContents.Select(c => new CourseContentDto{ Id = c.Id, Title = c.Title, Description = c.Description}).ToList(),
            CourseSkills = course.CourseSkills.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Name}).ToList(),
            TargetLearners = course.TargetLearners.Select(c => new SkillTargetDto{ Id = c.Id, Description = c.Description}).ToList()
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
            DurationHours = dto.DurationHours,
            CategoryId = dto.CategoryId,
            SellerId = dto.SellerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsApproved = false
        };

        if (dto.CourseContents != null)
        {
            foreach (var c in dto.CourseContents)
            {
                entity.CourseContents.Add(new CourseContent{ Title = c.Title, Description = c.Description});
            }
        }
        
        if (dto.CourseSkills != null)
        {
            foreach (var c in dto.CourseSkills)
            {
                entity.CourseSkills.Add(new CourseSkill{ Name = c.Description});
            }
        }
        
        if (dto.TargetLearners != null)
        {
            foreach (var c in dto.TargetLearners)
            {
                entity.TargetLearners.Add(new TargetLearner{ Description = c.Description});
            }
        }

        if (dto.Image != null)
        {
            entity.ImageUrl = await imageService.UploadImageAsync(dto.Image);
        }
        
        context.Courses.Add(entity);
        await context.SaveChangesAsync();
        
        return await GetByIdAsync(entity.Id) ?? throw new InvalidOperationException("Created but cannot retrieve");
    }

    public async Task<CourseDetailDto?> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var entity = await context.Courses
            .Include(c => c.CourseContents)
            .Include(c => c.CourseSkills)
            .Include(c => c.TargetLearners)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (entity == null) return null;
        
        entity.Title = dto.Title ?? entity.Title;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price ?? entity.Price;
        entity.Level = dto.Level ?? entity.Level;
        entity.TeacherName = dto.TeacherName ?? entity.TeacherName;
        entity.DurationHours = dto.DurationHours ?? entity.DurationHours;
        entity.CategoryId = dto.CategoryId  ?? entity.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // SYNC CourseContents
        SyncChildren<CourseContent, CourseContentDto>(
            existing: entity.CourseContents,
            incoming: dto.CourseContents ?? new List<CourseContentDto>(),
            addNew: d => new CourseContent{ Title = d.Title, Description = d.Description },
            updateExisting: (e, d) =>
            {
                e.Title = d.Description;
                e.Description = d.Description;
            },
            getId: d => d.Id
            );
        
        // SYNC CourseSkills
        SyncChildren<CourseSkill, SkillTargetDto>(
            existing: entity.CourseSkills,
            incoming: dto.CourseSkills ?? new List<SkillTargetDto>(),
            addNew: d => new CourseSkill{ Name = d.Description},
            updateExisting: (e, d) => e.Name = d.Description,
            getId: d => d.Id
        );
        
        // SYNC TargetLearners
        SyncChildren<TargetLearner, SkillTargetDto>(
            existing: entity.TargetLearners,
            incoming: dto.TargetLearners ?? new List<SkillTargetDto>(),
            addNew: d => new TargetLearner{ Description = d.Description},
            updateExisting: (e, d) => e.Description = d.Description,
            getId: d => d.Id
        );

        if (dto.Image != null)
        {
            if (!string.IsNullOrEmpty(entity.ImageUrl))
                await imageService.DeleteImageAsync(entity.ImageUrl);
            
            entity.ImageUrl = await imageService.UploadImageAsync(dto.Image);
        }
        
        await context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id);
    }

    private static void SyncChildren<TEntity, TDto>(
        ICollection<TEntity> existing,
        IEnumerable<TDto> incoming,
        Func<TDto, TEntity> addNew,
        Action<TEntity, TDto> updateExisting,
        Func<TDto, int> getId) where TEntity : class
    {
        var incomingList = incoming.ToList();
        var incomingIds = incomingList.Select(getId).Where(i => i != 0).ToHashSet();
        
        // remove those existing whose Id not in incomingIds
        var toRemove = existing
            .Where(e =>
            {
                var prop = e.GetType().GetProperty("Id");
                if (prop == null) return false;
                var val = (int)prop.GetValue(e)!;
                return !incomingIds.Contains(val);
            })
            .ToList();
        
        foreach (var r in toRemove) existing.Remove(r);
        
        // update existing and add new
        foreach (var dto in incomingList)
        {
            var id = getId(dto);
            if (id == 0)
            {
                existing.Add(addNew(dto));
            }
            else
            {
                // find existing by Id and update
                var found = existing.FirstOrDefault(e =>
                {
                    var prop = e.GetType().GetProperty("Id");
                    if (prop == null) return false;
                    var val = (int)prop.GetValue(e)!;
                    return val == id;
                });
                if (found != null) updateExisting(found, dto);
            }
        }
    }

    public async Task ApproveCourse(int courseId)
    {
        var course = await context.Courses.FindAsync(courseId);
        if (course == null) throw new NotFoundException("Course not found");
        if (course.IsApproved)
            throw new InvalidOperationException("Course already approved");
        course.IsApproved = true;
        if (await context.SaveChangesAsync() < 1) throw new InvalidOperationException("Save changes failed");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        context.Courses.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<CourseContentDto> AddCourseContentAsync(int courseId, CourseContentDto dto)
    {
        var course = await context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var content = new CourseContent { Title = dto.Title, Description = dto.Description, CourseId = courseId};
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
        var skill = new CourseSkill { Name = dto.Description, CourseId = courseId};
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
        var target = new TargetLearner { Description = dto.Description, CourseId = courseId};
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