using courses_buynsell_api.DTOs.User;
using courses_buynsell_api.Entities;

namespace courses_buynsell_api.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetAllUsersAsync();
    Task<UserDetailDto> GetUserByIdAsync(int id);
    Task DeleteUserAsync(DeleteUserRequest request);
    Task<UserDetailDto> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<UserDetailDto> AddAdminAsync(AddAdminRequest request);
    Task ChangeUserPasswordAsync(ChangeUserPasswordRequest request, int userId);
}
