using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;

namespace shappo0.Controllers
{
    public class KitchenController : Controller
    {
        private readonly IKitchenService _kitchenService;

        public KitchenController(IKitchenService kitchenService)
        {
            _kitchenService = kitchenService;
        }

        public IActionResult Index()
        {
            List<Kitchen> kitchen = _kitchenService.GetAll();
            return View(kitchen);
        }
    }
}
