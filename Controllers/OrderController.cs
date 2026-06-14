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
    [Authorize(Roles = "Waiter, Manager")]
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly ITableService _tableService;

        public OrderController(IMenuService menuService, IOrderService orderService, ITableService tableService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _tableService = tableService;
        }

        private static string GetCartKey(int tableNumber)
        {
            return $"WaiterCart_Table_{tableNumber}";
        }


        [Authorize]
        public IActionResult TakeOrder(int tableNumber, string? productType, string? menuType)
        {
            //Grabs the unique cartId for this table so we have the correct orderLines for this table
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? orderLines = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            //if there are no orderlines for this table, it will check the table status to make sure that it exists or is occupied. If not it will redirect the user back to the table overview page
            if (orderLines == null || orderLines.Count == 0)
            {
                Table? currentTable = _tableService.GetTableByNumber(tableNumber);

                if (currentTable == null || currentTable.Status != true)
                {
                    TempData["Error"] = $"Table {tableNumber} is currently empty or non existent.";
                    return RedirectToAction("Index", "Table");
                }
            }

            FilterItems filters = new FilterItems(productType, menuType, true);

            TakeOrderViewModel model = new(tableNumber)
            {
                ProductType = productType,
                MenuType = menuType,
                AllProducts = _menuService.GetFilteredProducts(filters, false)
            };

            if (orderLines != null) model.OrderLines = orderLines;
            
            return View(model);
        }
        
        public IActionResult AddToOrder(OrderActionViewModel request)
        {
            Product? selectedProduct = _menuService.GetByProductId(request.ProductId);

            if (selectedProduct != null)
            {
                //Grabs the exisiting orderLines from the cartId and if it's null it will create a new list
                string cartId = GetCartKey(request.TableNumber);
                List<OrderLine> cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId) ?? [];

                _orderService.AddProductToCart(cart, selectedProduct);

                HttpContext.Session.SetObject(cartId, cart);
                
            }
            return RedirectToAction("TakeOrder", new { tableNumber = request.TableNumber, productType = request.ProductType, menuType = request.MenuType });
        }

        public IActionResult IncreaseQuantity(OrderActionViewModel request)
        {
            string cartId = GetCartKey(request.TableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                bool wasSuccesful = _orderService.TryIncreaseQuantity(cart, request.ProductId);

                if (wasSuccesful)
                {
                    HttpContext.Session.SetObject(cartId, cart);
                }
                else
                {
                    TempData["Error"] = "Not enough stock to add this item";
                }
            }

            return RedirectToAction("TakeOrder", new { tableNumber = request.TableNumber, productType = request.ProductType, menuType = request.MenuType });
        }

        public IActionResult DecreaseQuantity(OrderActionViewModel request)
        {
            string cartId = GetCartKey(request.TableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                _orderService.DecreaseQuantity(cart, request.ProductId);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { tableNumber = request.TableNumber, productType = request.ProductType, menuType = request.MenuType });
        }

        public IActionResult RemoveLine(OrderActionViewModel request)
        {
            string cartId = GetCartKey(request.TableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null)
            {
                _orderService.RemoveLine(cart, request.ProductId);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { tableNumber = request.TableNumber, productType = request.ProductType, menuType = request.MenuType });
        }

        public IActionResult CancelOrder(int tableNumber)
        {
            string cartId = GetCartKey(tableNumber);
            HttpContext.Session.Remove(cartId);
            TempData["Success"] = $"Order for table {tableNumber} has been cancelled";
            return RedirectToAction("Index", "Table");
        }

        [HttpPost]
        public IActionResult AddNote(OrderActionViewModel request)
        {
            string cartId = GetCartKey(request.TableNumber);
            List<OrderLine>? cart = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            if (cart != null && request.Note != null)
            {
                _orderService.AddNote(cart, request.ProductId, request.Note);
                HttpContext.Session.SetObject(cartId, cart);
            }

            return RedirectToAction("TakeOrder", new { tableNumber = request.TableNumber, productType = request.ProductType, menuType = request.MenuType });
        }

        public IActionResult SaveOrder(int tableNumber)
        {
            string cartId = GetCartKey(tableNumber);
            List<OrderLine>? orderLines = HttpContext.Session.GetObject<List<OrderLine>>(cartId);

            string? staffIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Checks if the staffId can be parsed, if not it will redirect the user to the login page with an error message, this is to prevent orders being placed without a valid staffId which is required to link the order to the staff member who took it
            if (!int.TryParse(staffIdString, out int staffId))
            {
                TempData["Error"] = "Could not verify your staff identity. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            //Checks if the table is still occupied before saving the order, if not it will clear the cart and redirect the user to the table overview with an error message, this is to prevent orders being placed for tables that are no longer occupied
            Table? currentTable = _tableService.GetTableByNumber(tableNumber);
            if (currentTable == null || currentTable.Status != true)
            {
                // If the table is suddenly empty/invalid, clear their cart so they don't accidentally send it later!
                HttpContext.Session.Remove(cartId);
                TempData["Error"] = $"Table {tableNumber} is no longer occupied. The order was aborted.";
                return RedirectToAction("Index", "Table");
            }

            //Checks if there are orderLines to be saved, if not it will just redirect back to the table overview with an error message, this is to prevent empty orders being placed
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
