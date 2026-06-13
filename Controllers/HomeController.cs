using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace shappo0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Iedereen heeft een eigen home page
        [HttpGet]
        [Authorize] //moet op authorize staan, want anders is het mogelijk dat er geen claim is
        public IActionResult Index()
        {
            string? staffIdString = User.FindFirstValue(ClaimTypes.Role);

            if(!Enum.TryParse<RoleType>(staffIdString, out RoleType roleType))
            {
                TempData["Error"] = "Could not verify your staff identity. Please log in again.";
                RedirectToAction("Login", "Account");
            }

            switch (roleType)
            {
                case RoleType.Manager:
                    return RedirectToAction("Index", "Finance");
                case RoleType.Bar:
                    return RedirectToAction("Index", "Bar");
                case RoleType.Kitchen:
                    return RedirectToAction("Index", "Kitchen");
                default:
                    return RedirectToAction("Index", "Table");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
