using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    //[Authorize(Roles = "Kitchen")]
    public class KitchenController : Controller
    {
        private readonly IOrderService _orderService;

        public KitchenController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderService.GetRunningOrdersByType(ProductType.Main);
            return View(orders);
        }

        
        public IActionResult ViewOrder(int orderNumber)
        {
            List<OrderLine> lines = _orderService.GetOrderLinesByType(orderNumber, ProductType.Main);
            return View(lines);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _orderService.UpdateOrderStatus(orderNumber, status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderLineStatus(int orderLineId, int orderNumber, OrderStatus status)
        {
            _orderService.UpdateOrderLineStatus(orderLineId, status);

            return RedirectToAction("ViewOrder", new { orderNumber });
        }

        public IActionResult FinishedOrders()
        {
            List<Order> orders = _orderService.GetRunningOrders(OrderStatus.Served);
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status)
        {
            _orderService.UpdateCourseStatus(orderNumber, productType, status);

            return RedirectToAction("Vieworder", new { orderNumber });
        }
    }
}
