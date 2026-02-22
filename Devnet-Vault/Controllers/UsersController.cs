using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class UsersController : Controller
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
