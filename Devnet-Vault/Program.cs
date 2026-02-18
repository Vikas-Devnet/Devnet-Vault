using Application.DI;
using Application.Features.Common.Models;
using Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(option => option.AddPolicy("Allow_Vault_Client", policy =>
{
    policy.WithOrigins("https://abc.com")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
}));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, //No Extra Grace Time
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? throw new Exception("Issuer Key Not Found")))

    };
});

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers.ContentSecurityPolicy =

        // 'self' = only allow resources from same domain, protocol, and port
        "default-src 'self'; " +

        // 'self' = allow scripts from your own server
        "script-src 'self' https://cdn.jsdelivr.net https://code.jquery.com; " +

        // Allows CSS from your server and jsdelivr CDN
        "style-src 'self' https://cdn.jsdelivr.net; " +

        // 'self' = allow calls to same domain API
        "connect-src 'self' https://api.yourdomain.com; " +

        // 'self' = allow images from your server
        // data: = allow base64 images (example: inline images from database)
        "img-src 'self' data:; " +

        // 'none' = completely block iframe embedding
        "frame-ancestors 'none';";

    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseCors("Allow_Vault_Client");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
