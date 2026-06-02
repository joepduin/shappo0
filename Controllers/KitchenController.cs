using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.Enums;

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

        
        public IActionResult ViewOrder(int orderNumber)
        {
            List<OrderLine> lines = _kitchenService.GetOrderLines(orderNumber);
            return View(lines);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _kitchenService.UpdateOrderStatus(orderNumber, status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderLineStatus(int orderLineId, int orderNumber, OrderStatus status)
        {
            _kitchenService.UpdateOrderLineStatus(orderLineId, status);

            return RedirectToAction("ViewOrder", new { orderNumber });
        }

        public IActionResult FinishedOrders()
        {
            List<Kitchen> kitchen = _kitchenService.FinishedOrders();
            return View(kitchen);
        }

        [HttpPost]
        public IActionResult UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status)
        {
            _kitchenService.UpdateCourseStatus(orderNumber, productType, status);

            return RedirectToAction("Vieworder", new { orderNumber });
        }
    }
}
