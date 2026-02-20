using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class AdminController : Controller
{
    public IActionResult Auth()
    {
        return View("Views/Admin/Auth/Index.cshtml");
    }

    public IActionResult Home()
    {
        return View();
    }
}
