using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    [Authorize]
    public class BarController : Controller
    {
        private readonly IOrderService _orderService;

        public BarController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        
        public IActionResult Index()
        {
            List<OrderViewModel> orders = _orderService.GetRunningOrdersByType(ProductType.Drink);
            return View(orders);
        }

        
        public IActionResult ViewOrder(int orderNumber)
        {
            List<OrderLine> lines = _orderService.GetOrderLinesByType(orderNumber, ProductType.Drink);
            return View(lines);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderNumber, OrderStatus status)
        {
            _orderService.UpdateOrderStatus(orderNumber, status);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderLineStatus(OrderLine orderLine, OrderStatus status)
        {
            _orderService.UpdateOrderLineStatus(orderLine, status);

            return RedirectToAction("ViewOrder", new { orderLine.Order.OrderNumber });
        }

        public IActionResult FinishedOrders()
        {
            List<OrderViewModel> orders= _orderService.GetFinishedOrdersByType(ProductType.Drink);
            return View(orders);
        }
    }
}
