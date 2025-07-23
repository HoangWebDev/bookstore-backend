using BookStore.Data;
using BookStore.Handler;
using BookStore.IRepository;
using BookStore.Repository;
using BookStore.Request;
using BookStore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Đăng ký GeminiService
builder.Services.AddSingleton<GeminiService>();

builder.Services.Configure<CloudinaryRequest>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DapperContext>();

builder.Services.AddTransient<CustomJwtBearerHandler>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options => {});

builder.Services.AddSignalR();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin")); // 👈 Định nghĩa policy admin
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // ⚡ Thay bằng URL FE
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // 🔥 Quan trọng: Cho phép gửi cookies
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<ChatHub>("/chathub");

app.UseCors("AllowFrontend");  // ✅ Đặt ngay sau UseRouting()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
