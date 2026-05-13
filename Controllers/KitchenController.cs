using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Controllers
{
    public class KitchenController : Controller
    {
        private readonly IKitchenRepository _kitchenRepository; 
        public IActionResult Index()
        {
            List<Kitchen> kitchen = _kitchenRepository.GetAll();
            return View(kitchen);
        }
    }
}
