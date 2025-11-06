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
}