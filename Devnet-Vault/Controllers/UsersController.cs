using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class UsersController : Controller
{
    public IActionResult Auth()
    {
        return View("Views/Users/Auth/Index.cshtml");
    }

    public IActionResult Home()
    {
        return View();
    }
}
