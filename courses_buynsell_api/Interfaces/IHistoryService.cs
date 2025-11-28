using courses_buynsell_api.DTOs;
using courses_buynsell_api.DTOs.Course;

namespace courses_buynsell_api.Interfaces;

public interface IHistoryService
{
    Task<PagedResult<CourseListItemDto>> GetMyHistory(CourseQueryParameters query, int userId);
    Task<bool> AddHistoryAsync(int userId, int courseId);
    Task<bool> ClearHistoriesAsync(int userId);
    Task<bool> RemoveHistoryAsync(int userId, int courseId);
}