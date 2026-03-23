using Application.Features.Account.Dtos;
using Application.Features.Account.Interfaces;
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
        return View();
    }

    
}
