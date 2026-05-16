using Microsoft.AspNetCore.Mvc;
using URLShortener.Application.DTOs.Auth;
using URLShortener.Application.Interfaces;

namespace URLShortener.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = await _authService.LoginAsync(model);

            if (token == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, //Unsafe, I know, but needed for Angular client auto Auth
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("jwt", token, cookieOptions);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _authService.RegisterAsync(model, model.RegisterAsAdmin);

            if (!success)
            {
                ModelState.AddModelError("", "Email already exists");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }
    }
}