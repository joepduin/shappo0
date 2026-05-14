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

        public IActionResult Index()
        {
            List<Product> products = _menuService.GetProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddFoodItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFoodItem(Product product)
        {
            try
            {
                _menuService.AddMenuItem(product);

                TempData["ConfirmMessage"] = "User has been succesfully created";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: product could not be added";
                return View(product);
            }
        }

        [HttpGet]
        public IActionResult AddDrink()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDrink(Product product)
        {
            try
            {
                _menuService.AddMenuItem(product);

                TempData["ConfirmMessage"] = "User has been succesfully created";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: product could not be added";
                return View(product);
            }
        }

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
    }
}
