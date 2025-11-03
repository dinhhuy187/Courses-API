using Microsoft.EntityFrameworkCore;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Extensions;
using courses_buynsell_api.Data;
using courses_buynsell_api.Config;
using courses_buynsell_api.Interfaces;
using courses_buynsell_api.Services;
using courses_buynsell_api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Nạp .env TRƯỚC
Env.Load();

// 🔹 Thêm Environment Variables VÀO Configuration
builder.Configuration.AddEnvironmentVariables();


// 🔹 Kết nối PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION")));

// 🔹 Cấu hình JWT
var jwtSettings = new JwtSettings
{
    Key = Environment.GetEnvironmentVariable("JWT_KEY")!,
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!,
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!,
    ExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES")!)
};
builder.Services.Configure<JwtSettings>(opt =>
{
    opt.Key = jwtSettings.Key;
    opt.Issuer = jwtSettings.Issuer;
    opt.Audience = jwtSettings.Audience;
    opt.ExpiryMinutes = jwtSettings.ExpiryMinutes;
});

// 🔹 Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });





// ZaloPay Config
var zaloConfig = new ZaloPayConfig
{
    AppId = Environment.GetEnvironmentVariable("ZALOPAY_APP_ID")!,
    Key1 = Environment.GetEnvironmentVariable("ZALOPAY_KEY1")!,
    Key2 = Environment.GetEnvironmentVariable("ZALOPAY_KEY2")!,
    Endpoint = Environment.GetEnvironmentVariable("ZALOPAY_ENDPOINT")!,
    CallbackUrl = Environment.GetEnvironmentVariable("ZALOPAY_CALLBACK_URL")!,
};
builder.Services.AddSingleton(zaloConfig);

// Thêm sau phần JWT Settings
var vnPaySettings = new VnPaySettings
{
    TmnCode = Environment.GetEnvironmentVariable("VNPAY_TMNCODE")!,
    HashSecret = Environment.GetEnvironmentVariable("VNPAY_HASHSECRET")!,
    Url = Environment.GetEnvironmentVariable("VNPAY_URL")!,
    ReturnUrl = Environment.GetEnvironmentVariable("VNPAY_RETURNURL")!,
    Version = Environment.GetEnvironmentVariable("VNPAY_VERSION")!,
    Command = Environment.GetEnvironmentVariable("VNPAY_COMMAND")!
};
builder.Services.Configure<VnPaySettings>(opt =>
{
    opt.TmnCode = vnPaySettings.TmnCode;
    opt.HashSecret = vnPaySettings.HashSecret;
    opt.Url = vnPaySettings.Url;
    opt.ReturnUrl = vnPaySettings.ReturnUrl;
    opt.Version = vnPaySettings.Version;
    opt.Command = vnPaySettings.Command;
});

// Thêm vào phần Services (sau IFavoriteService)
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();





// 🔹 Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IZaloPayService, ZaloPayService>();

// 🔹 Controllers + Swagger
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// thêm middleware JWT
app.UseMiddleware<JwtMiddleware>();
app.UseErrorHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
