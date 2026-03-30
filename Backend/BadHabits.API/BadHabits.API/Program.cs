using BadHabits.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
// Đăng ký kết nối SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
// 1. Chống lặp vòng dữ liệu JSON
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// 2. Cấp Visa CORS cho Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
=======
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đọc chìa khóa bí mật từ appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChuoiBiMatMacDinhNeuKhongTimThay123456789";
var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

// Đăng ký hệ thống xác thực JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes)
        };
    });
<<<<<<< HEAD
=======
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
<<<<<<< HEAD
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
=======
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3
}

app.UseHttpsRedirection();

<<<<<<< HEAD
app.UseAuthentication(); 
=======
app.UseAuthentication();

app.UseCors("AllowFrontend"); // BẮT BUỘC phải nằm trên Authorization
>>>>>>> f48f68c0d002c86e7db8a5502522f12149ca16e3

app.UseAuthorization();

app.MapControllers();

app.Run();
