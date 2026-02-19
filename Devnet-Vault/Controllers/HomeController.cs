using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers
{
    public class HomeController(IOptions<HomeSettings> _homeOptions) : Controller
    {
        public HomeSettings homeSettings = _homeOptions.Value;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.PrivacyPolicyUpdatedDate = homeSettings.PrivacyPolicyUpdatedDate;
            ViewBag.SupportEmailAddress = homeSettings.SupportEmail;
            return View();
        }

        public IActionResult TermsConditions()
        {
            ViewBag.TermsConditionUpdatedDate = homeSettings.PrivacyPolicyUpdatedDate;
            ViewBag.SupportEmailAddress = homeSettings.SupportEmail;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
