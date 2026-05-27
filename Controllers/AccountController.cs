using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        //Stuurt door naar de inlog pagina
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        //Test om te kijken of de permissies werken.
        //naar de table pagina = werkend
        //naar accesdenied page = niet werkend
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult Test()
        {
            return RedirectToAction("Index", "Table");
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
            LoginModel CopyLoginModel = new LoginModel(loginModel.UserName, loginModel.Password);
            User user = _accountService.GetUserByCredentials(CopyLoginModel);

            if (user == null)
            {
                ViewBag.Message = "Onjuiste gebruikersnaam of wachtwoord.";
                return View(loginModel); 
            }
            else
            {
                SignInUser(user);
                //je gaat nu gelijk naar de table page. later ga je naar de pagina waarvoor je rol geschikt is.
                return RedirectToAction("Index", "Table");
            }
        }

        //maakt een Claim van de user om Authorize te gebruiken
        private void SignInUser(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(principal);
        }
    }
}
