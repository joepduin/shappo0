using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services;

namespace shappo0.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(string? productType, string? menuType, string? onMenu)
        {
            List<Product> products = _menuService.GetFilteredProducts(productType, menuType, onMenu);
            return View(products);
        }

        // The add methods for both food and drink items are separated, but both call the same AddItem method
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult AddFoodItem()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult AddFoodItem(Product product)
        {
            return AddItem(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult AddDrink()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult AddDrink(Product product)
        {
            return AddItem(product);
        }

        // Overarching method for adding an item (either drink or food), preventing double code
        public ActionResult AddItem(Product product)
        {
            try
            {
                // The price is divided by 1 + VAT percentage to get the price without VAT, which is what is stored in the database
                product.Price = product.Price / (1 + product.VATPercentage / 100);

                _menuService.AddMenuItem(product);

                TempData["ConfirmMessage"] = "Product has been succesfully created";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: product could not be added";
                return View(product);
            }
        }

        // Editing an item is done through the same view for both food and drink items. This gives the user full freedom to edit properties, including illegal combinations
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product? product = _menuService.GetByProductId((int)id);

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            try
            {
                _menuService.EditMenuItem(product);

                TempData["ConfirmMessage"] = "Product has been successfully edited";

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: product could not be edited.";
                return View(product);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult AdjustStock()
        {
            List<Product> products = _menuService.GetProducts();
            return View(products);
        }

        [HttpPost]
        public ActionResult AdjustStock(List<Product> adjustedProducts)
        {
            // First checks if there were any products selected, if not, it returns an error message
            if (adjustedProducts == null)
            {
                List<Product> products = _menuService.GetProducts();
                ViewBag.ErrorMessage = @"Exception occured: no products were selected.";
                return View(products);
            }
            try
            {
                foreach (Product product in adjustedProducts)
                {
                    _menuService.AdjustStock(product.ProductId, product.Stock);
                }
                return RedirectToAction("Index");

            }
            catch
            {
                // Annoyingly some double code, but catch method
                List<Product> products = _menuService.GetProducts();
                ViewBag.ErrorMessage = @"Exception occured: no products were selected.";
                return View(products);

            }
        }
    }
}
