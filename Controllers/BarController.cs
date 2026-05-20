using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;

namespace shappo0.Controllers
{
    public class BarController : Controller
    {
        private readonly IBarService _barService;

        public BarController(IBarService barService)
        {
            _barService = barService;
        }

        public IActionResult Index()
        {
            List<Bar> bar = _barService.GetAll();
            return View(bar);
        }
    }
}
