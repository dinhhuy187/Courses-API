using courses_buynsell_api.DTOs;
using courses_buynsell_api.DTOs.Course;

namespace courses_buynsell_api.Interfaces;

public interface ICourseService
{
    Task<PagedResult<CourseListItemDto>> GetCoursesAsync(CourseQueryParameters query);
    Task<CourseDetailDto?> GetByIdAsync(int id);
    Task<CourseDetailDto> CreateAsync(CreateCourseDto dto);
    Task<CourseDetailDto?> UpdateAsync(int id, UpdateCourseDto dto);
    Task<bool> DeleteAsync(int id);
    
    Task<ContentSkillTargetDto> AddCourseContentAsync(int courseId, ContentSkillTargetDto dto);
    Task<bool> RemoveCourseContentAsync(int courseId, int contentId);
    Task<ContentSkillTargetDto> AddCourseSkillAsync(int courseId, ContentSkillTargetDto dto);
    Task<bool> RemoveCourseSkillAsync(int courseId, int skillId);
    Task<ContentSkillTargetDto> AddTargetLearnerAsync(int courseId, ContentSkillTargetDto dto);
    Task<bool> RemoveTargetLearnerAsync(int courseId, int learnerId);
}