
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Roomantic_BoardingHouseSystem.Models;
using Roomantic_BoardingHouseSystem.Services;
using System.Security.Claims;

namespace Roomantic_BoardingHouseSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<AdminAccountModel> _admins;
        private readonly IMongoCollection<TenantModel> _tenants;

        public HomeController(MongoDbService db)
        {
            _admins = db.AdminAccounts;
            _tenants = db.Tenants;
        }

        [Authorize] // kung gusto mong private ang Home; remove if public
        public IActionResult Home()
        {
            return View();
        }

        // ==============================
        // LOGIN VIEW
        // ==============================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // ==============================
        // LOGIN POST
        // ==============================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Normalize username kung gusto mo case-insensitive
            var inputUsername = model.Username.Trim();

            // 1) Try Admin
            var admin = await _admins
                .Find(a => a.Username == inputUsername /* && a.IsActive */)
                .FirstOrDefaultAsync();

            if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.PasswordHash))
            {
                await SignInUser(admin.Id, admin.Username, "Admin");
                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Admin");
            }

            // 2) Try Tenant
            var tenant = await _tenants
                .Find(t => t.Username == inputUsername /* && t.IsActive */)
                .FirstOrDefaultAsync();

            if (tenant != null && BCrypt.Net.BCrypt.Verify(model.Password, tenant.PasswordHash))
            {
                await SignInUser(tenant.Id, tenant.Username, "Tenant");
                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Dashboard", "Tenant");
            }

            // INVALID LOGIN
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        // ==============================
        // SIGN IN USER
        // ==============================
        private async Task SignInUser(string? id, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id ?? string.Empty),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,   // “Remember me” behavior (session persists)
                    AllowRefresh = true
                    // ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) // optional override
                });
        }

        // ==============================
        // LOGOUT
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Optional GET logout (less secure; only if needed)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
