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
        try
        {
            var result = await _authService.RegisterAsync(dto);

            // KHÔNG set cookie khi register (chưa có refresh token)
            return Ok(new
            {
                message = "Registration successful. Please check your email to verify your account.",
                email = result.Email,
                fullName = result.FullName,
                role = result.Role
            });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedException ex) // email / username đã tồn tại
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);

            // Chỉ set cookie khi login thành công
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenCookie(result.RefreshToken);
            }

            return Ok(result);
        }
        catch (UnauthorizedException ex) // sai mật khẩu / chưa verify email
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (NotFoundException ex) // không tồn tại user
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
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
        try
        {
            await _authService.VerifyEmailAsync(token);

            // === HTML: XÁC THỰC THÀNH CÔNG ===
            var successHtml = @"
            <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Email Verified</title>
                </head>
                <body style='font-family: sans-serif; text-align:center; padding-top: 50px;'>
                    <h2 style='color: #28a745;'>✔ Xác thực thành công</h2>
                    <p>Email của bạn đã được xác thực. Bạn có thể đăng nhập ngay bây giờ.</p>
                </body>
            </html>";

            return Content(successHtml, "text/html");
        }
        catch (Exception)
        {
            // === HTML: XÁC THỰC THẤT BẠI ===
            var errorHtml = @"
            <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Email Verification Failed</title>
                </head>
                <body style='font-family: sans-serif; text-align:center; padding-top: 50px;'>
                    <h2 style='color: #d9534f;'>❌ Xác thực thất bại</h2>
                    <p>Liên kết xác thực không hợp lệ hoặc đã hết hạn.</p>
                </body>
            </html>";

            return Content(errorHtml, "text/html");
        }
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        try
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { message = "If the email exists, a password reset OTP has been sent." });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    [HttpPost("check-otp")]
    public async Task<IActionResult> CheckOTP(CheckOTPDto dto)
    {
        try
        {
            await _authService.CheckOTPAsync(dto.Email, dto.OTP);
            return Ok(new { message = "OTP is valid" });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        try
        {
            await _authService.ResetPasswordAsync(dto.OTP, dto.NewPassword, dto.Email);
            return Ok(new { message = "Password reset successfully. Please login with your new password." });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
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

    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendEmailDto dto)
    {
        await _authService.ResendVerificationEmailAsync(dto.Email);

        return Ok(new
        {
            message = "If the account exists and is not verified, a new verification email has been sent."
        });
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