using Microsoft.AspNetCore.Mvc;

namespace shappo0.Controllers
{
    public class FinanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
