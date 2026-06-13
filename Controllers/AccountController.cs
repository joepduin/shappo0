using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using shappo0.Services;
using System.Security.Claims;

namespace shappo0.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        //account pagina
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            //safely fetch the staffId from the claim, if it fails, redirect to login
            string? roleString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!int.TryParse(roleString, out int staffId))
            {
                TempData["ErrorMessage"] = "User not logged in, please log in to view your account.";
                return RedirectToAction("Login", "Account");
            }

            Staff? staff = _accountService.GetStaff(staffId);
            return View(staff);
        }
        
        //de inlog pagina
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //view als de gebruiker geen permissies heeft voor een pagina
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        //Logt de gebruiker uit, moet wel eerst ingelogged zijn.
        [Authorize]
        [HttpGet]
        public IActionResult LogOff()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        //zoekt via userreportie naar de gebruikt. krijgt een lege user bij een fout.
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            Staff? staff = _accountService.GetStaffByCredentials(loginModel);

            if (staff == null)
            {
                ViewBag.ErrorMessage = "Incorrect username or password.";
                loginModel.Password = string.Empty;
                return View(loginModel); 
            }
            
            SignInStaff(staff);
            return RedirectToAction("Index", "Home"); //als je naar deze pagina gaat ga je naar je eigen home page
        }


        //haalt de Claim op van de user om Authorize te gebruiken
        private void SignInStaff(Staff staff)
        {
            List<Claim> claims = _accountService.GetUserClaims(staff);

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(principal);
        }
    }
}
