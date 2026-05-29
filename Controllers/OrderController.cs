using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Extensions;
using shappo0.Models;
using shappo0.Services;
using shappo0.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace shappo0.Controllers
{
    [Authorize (Roles="Waiter")]
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        
        public OrderController(IMenuService menuService, IOrderService orderService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        private string GetCartKey(int tableNumber)
        {
            return $"WaiterCart_Table_{tableNumber}";
        }


        [Authorize]
        public IActionResult TakeOrder(int tableNumber, string? productType, string? menuType)
        {
            if (tableNumber <= 0)
            {
                TempData["Error"] = "Please select a table first.";
                return RedirectToAction("Index", "Table");
            }
            
            TakeOrderViewModel model = new(tableNumber);

            List<Product> products = _menuService.GetFilteredProducts(productType, menuType, "true");

            //Grabs the unique cartId for this table so we have the correct orderLines for this table
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? orderLines = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (orderLines != null)
            {
                model.OrderLines = orderLines;
            }

            model.AllProducts = products;

            return View(model);
        }

        public IActionResult AddToOrder(int tableNumber, int productId, string? productType, string? menuType)
        {
            Product selectedProduct = _menuService.GetByProductId(productId);

            //Grabs the exisiting orderLines from the cartId and if it's null it will create a new list
            string cartId = GetCartKey(tableNumber);
            List<OrderLine> cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId) ?? new List<OrderLine>();

            _orderService.AddProductToCart(cart, selectedProduct);

            HttpContext.Session.SetObject(cartId, cart);

            return RedirectToAction("TakeOrder", new {tableNumber = tableNumber, productType = productType, menuType = menuType });
        }

        public IActionResult IncreaseQuantity(int tableNumber, int productId, string? productType, string? menuType)
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                bool wasSuccesful = _orderService.TryIncreaseQuantity(cart, productId);

                if (wasSuccesful)
                {
                    HttpContext.Session.SetObject(cartId, cart);
                }
                else
                {
                    TempData["Error"] = "Not enough stock to add this item";
                }
            }
            return RedirectToAction("TakeOrder", new { tableNumber = tableNumber, productType = productType, menuType = menuType });
        }

        public IActionResult DecreaseQuantity(int tableNumber, int productId, string? productType, string? menuType)
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if(cart != null)
            {
                _orderService.DecreaseQuantity(cart, productId);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { TableNumber = tableNumber, ProductType = productType, MenuType = menuType });
        }

        public IActionResult CancelOrder(int tableNumber)
        {
            string cartId = GetCartKey(tableNumber);
            HttpContext.Session.Remove(cartId);
            TempData["Success"] = $"Order for table {tableNumber} has been cancelled";
            return RedirectToAction("Index", "Table");
        }

        public IActionResult RemoveLine(int tableNumber, int productId, string? productType, string? menuType) 
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                _orderService.RemoveLine(cart, productId);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { tableNumber = tableNumber, menuType = menuType, productType = productType });
        }

        [HttpPost]
        public IActionResult AddNote(int tableNumber, int productId, string note, string? productType, string? menuType)
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                _orderService.AddNote(cart, productId, note);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { tableNumber = tableNumber, menuType = menuType,  productType = productType});
        }

        public IActionResult SaveOrder(int tableNumber)
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? orderLines = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            int staffId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (orderLines != null && orderLines.Count > 0)
            {
                _orderService.CreateOrder(tableNumber, staffId, orderLines);

                HttpContext.Session.Remove(cartId);
                TempData["Success"] = $"Order for table {tableNumber} has been succesfully placed";
            }
            else
            {
                TempData["Error"] = "Can't save order if there are no orderlines to be saved";
            }

            return RedirectToAction("Index", "Table");
        }
    }
}
