using Infrastructure.Configurations;
using Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add all infra + app services via extension
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddInfrastructure(builder.Configuration);

// Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", opt => opt.LoginPath = "/Auth/Login");

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.Run();
