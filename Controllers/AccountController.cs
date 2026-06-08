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
            int staffId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            LoginModel CopyLoginModel = new LoginModel(loginModel.Name, loginModel.Password);
            Staff? staff = _accountService.GetStaffByCredentials(CopyLoginModel);

            if (staff == null)
            {
                ViewBag.Message = "Onjuiste gebruikersnaam of wachtwoord.";
                return View(loginModel); 
            }
            else
            {
                SignInStaff(staff);
                return RedirectToAction("Index", "Home"); //als je naar deze pagina gaat ga je naar je eigen home page
            }
        }


    //maakt een Claim van de user om Authorize te gebruiken
    private void SignInStaff(Staff staff)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
                new Claim(ClaimTypes.Name, staff?.Name ?? string.Empty),
                new Claim(ClaimTypes.Role, staff?.Role.ToString() ?? string.Empty)
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(principal);
        }
    }
}
