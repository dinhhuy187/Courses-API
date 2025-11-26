using courses_buynsell_api.DTOs.Cart;
using courses_buynsell_api.DTOs.Course;

namespace courses_buynsell_api.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CourseListItemDto>> GetCartAsync(int userId);
    Task<CartDto> AddItemAsync(int userId, int courseId);
    Task<bool> RemoveItemAsync(int userId, int itemId);
    Task<bool> ClearCartAsync(int userId);
}