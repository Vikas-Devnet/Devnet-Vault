using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers;

public class UsersController() : Controller
{
    public IActionResult Auth()
    {
        return View();
    }

    public IActionResult Home()
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        ViewBag.LoggedUserFullName = username;
        ViewData["ActivePage"] = "home";
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }
}
