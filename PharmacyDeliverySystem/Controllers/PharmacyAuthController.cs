using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PharmacyDeliverySystem.Controllers
{
    public class PharmacyAuthController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyAuthController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // ============= LOGIN (GET) ============
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new PharmacyLoginViewModel());
        }

        // ============= LOGIN (POST) ============
        [HttpPost]
        public IActionResult Login(PharmacyLoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var pharmacy = _context.Pharmacies.FirstOrDefault(p => p.Email == model.Email);

            if (pharmacy == null || pharmacy.PasswordHash != model.Password)
            {
                ViewBag.Error = "Invalid email or password";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, pharmacy.Name),
                new Claim(ClaimTypes.Email, pharmacy.Email),
                new Claim(ClaimTypes.Role, "Pharmacy")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            // لما صفحات ال Pharmacy تجهز غيري الـ Redirect هنا
            return RedirectToAction("Index", "Home");
        }

        // ============= REGISTER (GET) ============
        [HttpGet]
        public IActionResult Register()
        {
            return View(new PharmacyRegisterViewModel());
        }

        // ============= REGISTER (POST) ============
        [HttpPost]
        public IActionResult Register(PharmacyRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = _context.Pharmacies.Any(p => p.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
                return View(model);
            }

            var pharmacy = new Pharmacy
            {
                Name = model.Name,
                LicenceNo = model.LicenceNo,
                TaxId = model.TaxId,
                Email = model.Email,
                PasswordHash = model.Password // عدلي لو فيه hashing
            };

            _context.Pharmacies.Add(pharmacy);
            _context.SaveChanges();

            // Login تلقائي بعد التسجيل
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, pharmacy.Name),
                new Claim(ClaimTypes.Email, pharmacy.Email),
                new Claim(ClaimTypes.Role, "Pharmacy")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // غيريه بعدين لصفحة الـ dashboard بتاعة الصيدلية
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
