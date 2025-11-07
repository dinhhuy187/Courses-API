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
        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.CourseContents)
            .Include(c => c.CourseSkills)
            .Include(c => c.TargetLearners)
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
            CategoryId = course.CategoryId,
            IsApproved = course.IsApproved,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            CourseContents = course.CourseContents.Select(c => new ContentSkillTargetDto{ Id = c.Id, Description = c.Title}).ToList(),
            CourseSkills = course.CourseSkills.Select(c => new ContentSkillTargetDto{ Id = c.Id, Description = c.Name}).ToList(),
            TargetLearners = course.TargetLearners.Select(c => new ContentSkillTargetDto{ Id = c.Id, Description = c.Description}).ToList()
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

        if (dto.CourseContents != null)
        {
            foreach (var c in dto.CourseContents)
            {
                entity.CourseContents.Add(new CourseContent{ Title = c.Description});
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
        
        _context.Courses.Add(entity);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(entity.Id) ?? throw new InvalidOperationException("Created but cannot retrieve");
    }

    public async Task<CourseDetailDto?> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var entity = await _context.Courses
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
        entity.ImageUrl = dto.ImageUrl;
        entity.DurationHours = dto.DurationHours ?? entity.DurationHours;
        entity.CategoryId = dto.CategoryId  ?? entity.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        
        // SYNC CourseContents
        SyncChildren<CourseContent, ContentSkillTargetDto>(
            existing: entity.CourseContents,
            incoming: dto.CourseContents ?? new List<ContentSkillTargetDto>(),
            addNew: d => new CourseContent{ Title = d.Description},
            updateExisting: (e, d) => e.Title = d.Description,
            getId: d => d.Id
            );
        
        // SYNC CourseSkills
        SyncChildren<CourseSkill, ContentSkillTargetDto>(
            existing: entity.CourseSkills,
            incoming: dto.CourseSkills ?? new List<ContentSkillTargetDto>(),
            addNew: d => new CourseSkill{ Name = d.Description},
            updateExisting: (e, d) => e.Name = d.Description,
            getId: d => d.Id
        );
        
        // SYNC TargetLearners
        SyncChildren<TargetLearner, ContentSkillTargetDto>(
            existing: entity.TargetLearners,
            incoming: dto.TargetLearners ?? new List<ContentSkillTargetDto>(),
            addNew: d => new TargetLearner{ Description = d.Description},
            updateExisting: (e, d) => e.Description = d.Description,
            getId: d => d.Id
        );
        
        await _context.SaveChangesAsync();
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

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return false;
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ContentSkillTargetDto> AddCourseContentAsync(int courseId, ContentSkillTargetDto dto)
    {
        var course = await _context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var content = new CourseContent { Title = dto.Description, CourseId = courseId};
        _context.CourseContents.Add(content);
        await _context.SaveChangesAsync();
        dto.Id = content.Id;
        return dto;
    }

    public async Task<bool> RemoveCourseContentAsync(int courseId, int contentId)
    {
        var c = await _context.CourseContents.FirstOrDefaultAsync(x => x.Id == contentId && x.CourseId == courseId);
        if (c == null) return false;
        _context.CourseContents.Remove(c);
        await _context.SaveChangesAsync();
        return true;    
    }

    public async Task<ContentSkillTargetDto> AddCourseSkillAsync(int courseId, ContentSkillTargetDto dto)
    {
        var course = await _context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var skill = new CourseSkill { Name = dto.Description, CourseId = courseId};
        _context.CourseSkills.Add(skill);
        await _context.SaveChangesAsync();
        dto.Id = skill.Id;
        return dto;
    }

    public async Task<bool> RemoveCourseSkillAsync(int courseId, int skillId)
    {
        var c = await _context.CourseSkills.FirstOrDefaultAsync(x => x.CourseId == courseId && x.Id == skillId);
        if (c == null) return false;
        _context.CourseSkills.Remove(c);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ContentSkillTargetDto> AddTargetLearnerAsync(int courseId, ContentSkillTargetDto dto)
    {
        var course = await _context.Courses.FindAsync(courseId) ?? throw new KeyNotFoundException("Course not found");
        var target = new TargetLearner { Description = dto.Description, CourseId = courseId};
        _context.TargetLearners.Add(target);
        await _context.SaveChangesAsync();
        dto.Id = target.Id;
        return dto;     
    }

    public async Task<bool> RemoveTargetLearnerAsync(int courseId, int learnerId)
    {
        var t = await _context.TargetLearners.FirstOrDefaultAsync(x => x.CourseId == courseId && x.Id == learnerId);
        if (t == null) return false;
        _context.TargetLearners.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }
}