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

        /* ==================== LOGIN ==================== */

        [HttpGet]
        public IActionResult Login() => View(new PharmacyLoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(PharmacyLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var pharmacy = _context.Pharmacies
                .FirstOrDefault(p => p.Email == model.Email && p.PasswordHash == model.Password);
            // TODO: استبدل مقارنة الباسورد بـ Hashing حقيقي بعدين

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

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = System.DateTime.UtcNow.AddHours(8)
                });

            // بعد اللوجين يروح على صفحة الشات الخاصة بالصيدلي
            return RedirectToAction("Chats", "PharmacyChat");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        /* ==================== REGISTER ==================== */

        // GET: PharmacyAuth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new PharmacyRegisterViewModel());
        }

        // POST: PharmacyAuth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(PharmacyRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // تأكد إن الإيميل مش متسجل قبل كده
            bool emailExists = _context.Pharmacies.Any(p => p.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            // إنشاء كيان الصيدلية الجديد
            var pharmacy = new Pharmacy
            {
                Name = model.Name,
                Email = model.Email,
                // مؤقتاً بنخزن الباسورد زي ما هو – المفروض تستخدم Hashing بعدين
                PasswordHash = model.Password
            };

            _context.Pharmacies.Add(pharmacy);
            _context.SaveChanges();

            // بعد الريجستر نرجّع الصيدلي لصفحة اللوجين بتاعته
            return RedirectToAction("Login", "PharmacyAuth");
        }
    }
}
