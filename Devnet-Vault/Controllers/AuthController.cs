namespace Presentation.Controllers;

using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[Controller]")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await auth.Login(dto);
        if (!result.IsSuccess) return View(dto);

        // Cookie login
        HttpContext.Response.Cookies.Append("AuthCookie", result.Token!);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Signup(SignupDto dto)
    {
        var result = await auth.Signup(dto);
        if (!result.IsSuccess) return View(dto);

        HttpContext.Response.Cookies.Append("AuthCookie", result.Token!);
        return RedirectToAction("Index", "Home");
    }
}
