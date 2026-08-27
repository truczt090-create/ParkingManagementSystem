using Microsoft.AspNetCore.Authentication.Cookies;
using ParkingManagement.Web.Helpers;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// THÊM MỚI: bắt buộc phải có để dùng Session
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();

#region HttpClient
builder.Services.AddHttpClient("ParkingAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7264/api/v1/"); 
}).AddHttpMessageHandler<AuthTokenHandler>();   
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

#endregion

#region Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IParkingLotService, ParkingLotService>();
builder.Services.AddScoped<IBookingService, BookingService>();
#endregion
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IChatbotService, ChatbotService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();          
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();