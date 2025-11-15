using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.User;
using courses_buynsell_api.Exceptions;
using courses_buynsell_api.Interfaces;
using courses_buynsell_api.Helpers;

using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public UserService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber ?? String.Empty,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
            return users;
        }

        public async Task<UserDetailDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDetailDto
                {
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role,
                    AvatarUrl = u.AvatarUrl // thêm avatar
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found.");
            }

            return user;
        }


        public async Task DeleteUserAsync(DeleteUserRequest request)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID {request.Id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDetailDto> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found.");
            }

            // Update text fields
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            // Update avatar nếu có file mới
            if (request.Avatar != null)
            {
                // Xóa avatar cũ nếu có
                if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
                {
                    await _imageService.DeleteImageAsync(user.AvatarUrl);
                }

                // Upload avatar mới
                var avatarUrl = await _imageService.UploadImageAsync(request.Avatar);
                user.AvatarUrl = avatarUrl;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new UserDetailDto
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            };
        }


        public async Task<UserDetailDto> AddAdminAsync(AddAdminRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("A user with this email already exists.");
            }

            var newAdmin = new Entities.User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();

            return new UserDetailDto
            {
                FullName = newAdmin.FullName,
                Email = newAdmin.Email,
                PhoneNumber = newAdmin.PhoneNumber,
                Role = newAdmin.Role
            };
        }

        public async Task ChangeUserPasswordAsync(ChangeUserPasswordRequest request, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            if (!PasswordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                throw new BadRequestException("Current password is incorrect.");
            }

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}