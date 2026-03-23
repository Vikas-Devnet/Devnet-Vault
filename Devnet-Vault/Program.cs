using Application;
using Application.Features.Common.Models;
using Infrastructure;
using Presentation;
using Presentation.Extensions;
using Presentation.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<HomeSettings>(builder.Configuration.GetSection("HomeSettings"));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();

app.UseCors("Allow_Vault_Client");
app.UseAuthentication(); 
app.UseAuthorization(); 
app.UsePresentation();
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();