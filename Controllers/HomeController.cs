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
            RoleType roleType = Enum.Parse<RoleType>(User.FindFirstValue(ClaimTypes.Role));
            if (roleType == RoleType.Manager) return RedirectToAction("Index", "Finance");
            else if (roleType == RoleType.Bar) return RedirectToAction("Index", "Bar");
            else if (roleType == RoleType.Kitchen) return RedirectToAction("Index", "Kitchen");
            else return RedirectToAction("Index", "Table");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
