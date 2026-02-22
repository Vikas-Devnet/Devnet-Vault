using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class AdminController : Controller
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
