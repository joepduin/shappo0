using Microsoft.AspNetCore.Mvc;

namespace shappo0.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
