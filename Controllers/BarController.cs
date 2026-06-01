using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.Enums;

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
