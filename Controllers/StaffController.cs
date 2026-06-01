using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services;
namespace shappo0.Controllers
{
    [Authorize(Roles = "Manager")]
    public class StaffController : Controller
    {
        private readonly IAccountService _accountService;

        public StaffController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Staff> staff = _accountService.GetAllStaff();
            return View(staff);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(Staff staff)
        {
            try
            {
                _accountService.AddStaff(staff);

                TempData["ConfirmMessage"] = "Staff member has been succesfully created";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: staff member could not be added";
                return View(staff);
            }
        }

        [HttpGet]
        public IActionResult EditStaff(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Staff? staff = _accountService.GetStaff((int)id);

            return View(staff);
        }

        [HttpPost]
        public ActionResult EditStaff(Staff staff)
        {
            try
            {
                _accountService.EditStaff(staff);

                TempData["ConfirmMessage"] = "Staff member has been successfully edited";

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.ErrorMessage = @"Exception occured: staff member could not be edited.";
                return View(staff);
            }
        }
    }
}
