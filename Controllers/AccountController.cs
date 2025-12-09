using System.Security.Claims;
using BudgetTracker.Models;
using BudgetTracker.Models.ViewModels;
using BudgetTracker.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBudgetRepository _budgetRepository;

        public AccountController(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _budgetRepository.GetUserByUsername(model.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _budgetRepository.GetUserByUsername(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _budgetRepository.RegisterUser(user);

                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
