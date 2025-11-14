using courses_buynsell_api.DTOs.Auth;
using courses_buynsell_api.Exceptions;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using courses_buynsell_api.Data;

namespace courses_buynsell_api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        // ✅ KHÔNG set cookie khi register (vì chưa có refreshToken)
        // User cần verify email trước khi login
        return Ok(new
        {
            message = "Registration successful. Please check your email to verify your account.",
            email = result.Email,
            fullName = result.FullName,
            role = result.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        // ✅ Chỉ set cookie khi login thành công
        if (!string.IsNullOrEmpty(result.RefreshToken))
        {
            SetRefreshTokenCookie(result.RefreshToken);
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("No refresh token found.");

        var result = await _authService.RefreshTokenAsync(refreshToken);

        // ✅ Update cookie với refreshToken mới
        if (!string.IsNullOrEmpty(result.RefreshToken))
        {
            SetRefreshTokenCookie(result.RefreshToken);
        }

        return Ok(result);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        await _authService.VerifyEmailAsync(token);
        return Ok(new { message = "Email verified successfully. You can now login." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await _authService.ForgotPasswordAsync(dto.Email);
        return Ok(new { message = "If the email exists, a password reset OTP has been sent." });
    }

    [HttpPost("check-otp")]
    public async Task<IActionResult> CheckOTP(CheckOTPDto dto)
    {
        await _authService.CheckOTPAsync(dto.Email, dto.OTP);
        return Ok(new { message = "OTP is valid" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        await _authService.ResetPasswordAsync(dto.OTP, dto.NewPassword, dto.Email);
        return Ok(new { message = "Password reset successfully. Please login with your new password." });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // ✅ Xóa cookie khi logout
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // Quan trọng cho cross-origin
            Path = "/"
        });

        return Ok(new { message = "Logged out successfully" });
    }

    //[Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { message = "Invalid token" });

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid user ID" });

        var user = await _authService.GetCurrentUserAsync(userId);

        return Ok(user);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,    // ✅ Ngăn JavaScript đọc cookie
            Secure = true,      // ✅ Chỉ gửi qua HTTPS (production)
            SameSite = SameSiteMode.None, // ✅ Cho phép cross-origin (FE/BE khác domain)
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"          // ✅ Cookie khả dụng cho toàn bộ app
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}