using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;

namespace PharmacyDeliverySystem.Controllers
{
    public class PharmacyAuthController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyAuthController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View(new PharmacyLoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(PharmacyLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var pharmacy = _context.Pharmacies.FirstOrDefault(p => p.Email == model.Email && p.PasswordHash == model.Password);
            if (pharmacy == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, pharmacy.Name),
                new Claim(ClaimTypes.Email, pharmacy.Email),
                new Claim(ClaimTypes.Role, "Pharmacy"),
                new Claim("PharmacyId", pharmacy.PharmId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in and persist the cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = System.DateTime.UtcNow.AddHours(8)
                });

            // Redirect to chat page after login
            return RedirectToAction("Chats", "PharmacyChat");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
