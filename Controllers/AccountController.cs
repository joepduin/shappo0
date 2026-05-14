using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using System.Security.Claims;

namespace shappo0.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public AccountController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        //Test pagina waar je komt na inloggen
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //Test om te kijken of de permissies werken.
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult Test()
        {
            return RedirectToAction("Index", "Account");
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
            User user = _usersRepository.GetUserByCredentials(loginModel);

            if (user == null)
            {
                ViewBag.Message = "Onjuiste gebruikersnaam of wachtwoord.";
                return View(loginModel); 
            }
            else
            {
                SignInUser(user);

                return RedirectToAction("Index", "Account");
            }
        }

        //maakt een Claim van de user om Authorize te gebruiken
        private void SignInUser(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Gender, user.Sex?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Expired, user.IsEmployed ? "1" : "0") //moet een string zijn 1 is true 0 is false
            };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(principal);
        }
    }
}
