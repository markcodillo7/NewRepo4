
using Microsoft.AspNetCore.Authentication.Cookies;
using Roomantic_BoardingHouseSystem.Models;
using Roomantic_BoardingHouseSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Mongo Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// MongoDb service
builder.Services.AddSingleton<MongoDbService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(6);
    });

// Authorization
builder.Services.AddAuthorization();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}"
);

app.Run();
