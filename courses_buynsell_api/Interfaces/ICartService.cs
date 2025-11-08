using courses_buynsell_api.DTOs.Cart;

namespace courses_buynsell_api.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(int userId);
    Task<CartDto> AddItemAsync(AddCartItemDto dto);
    Task<bool> RemoveItemAsync(int userId, int itemId);
    Task<bool> ClearCartAsync(int userId);
}