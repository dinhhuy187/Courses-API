using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using courses_buynsell_api.Config;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using courses_buynsell_api.Data;

namespace courses_buynsell_api.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var isValid = AttachUserToContext(context, dbContext, token);
                if (!isValid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired token");
                    return; // ❌ Dừng middleware chain ở đây
                }
            }

            await _next(context);
        }

        private bool AttachUserToContext(HttpContext context, AppDbContext dbContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // không cho phép lệch thời gian
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdStr = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

                if (int.TryParse(userIdStr, out var userId))
                {
                    context.Items["UserId"] = userId;
                    return true;
                }

                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("⚠️ Token đã hết hạn");
                return false; // 401
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Token không hợp lệ: {ex.Message}");
                return false; // 401
            }
        }
    }
}
