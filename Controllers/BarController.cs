using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Controllers
{
    public class BarController : Controller
    {
        private readonly IBarRepository _barRepository;
        public IActionResult Index()
        {
            List<Bar> bar = _barRepository.GetAll();
            return View(bar);
        }
    }
}
