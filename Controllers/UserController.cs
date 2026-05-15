using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services;
namespace shappo0.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<User> users = _accountService.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            try
            {
                _accountService.AddUser(user);

                TempData["ConfirmMessage"] = "User has been succesfully created";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: user could not be added";
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User? user = _accountService.GetUser((int)id);

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(User user)
        {
            try
            {
                _accountService.EditUser(user);

                TempData["ConfirmMessage"] = "User has been successfully edited";

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: user could not be edited.";
                return View(user);
            }
        }
    }
}
