using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
   // [Authorize(Roles = "Bar")]
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


            KitchenBarViewModel bars = new KitchenBarViewModel(bar);

            return View(bars);
        }

        
        public IActionResult ViewOrder(int orderNumber)
        {
            List<OrderLine> lines = _barService.GetOrderLines(orderNumber);
            return View(lines);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _barService.UpdateOrderStatus(orderNumber, status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderLineStatus(int orderLineId, int orderNumber, OrderStatus status)
        {
            _barService.UpdateOrderLineStatus(orderLineId, status);

            return RedirectToAction("ViewOrder", new { orderNumber });
        }

        public IActionResult FinishedOrders()
        {
            List<Bar> bars = _barService.FinishedOrders();
            return View(bars);
        }
    }
}
