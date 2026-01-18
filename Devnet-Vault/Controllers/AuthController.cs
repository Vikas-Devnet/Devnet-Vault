namespace Devnet_Vault.Controllers;

using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

public class AuthController(AuthService auth) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult Signup() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await auth.Login(dto);
        if (!result.IsSuccess) return View(dto);

        // Cookie login
        HttpContext.Response.Cookies.Append("AuthCookie", result.Token!);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupDto dto)
    {
        var result = await auth.Signup(dto);
        if (!result.IsSuccess) return View(dto);

        HttpContext.Response.Cookies.Append("AuthCookie", result.Token!);
        return RedirectToAction("Index", "Home");
    }
}
