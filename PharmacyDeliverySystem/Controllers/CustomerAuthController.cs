using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PharmacyDeliverySystem.Controllers
{
    public class CustomerAuthController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public CustomerAuthController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // =======================
        //       LOGIN (GET)
        // =======================
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CustomerLoginViewModel());
        }

        // =======================
        //       LOGIN (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomerLoginViewModel model, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill Email and Password correctly.";
                return View(model);
            }

            var email = model.Email?.Trim();
            var password = model.Password?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View(model);
            }

            // 1) نحاول الأول كـ Pharmacy
            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == email);

            if (pharmacy != null)
            {
                var dbPassword = pharmacy.PasswordHash?.Trim();

                if (!string.Equals(dbPassword, password, StringComparison.Ordinal))
                {
                    ViewBag.Error = "Wrong password.";
                    return View(model);
                }

                var pharmacyClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,  pharmacy.Name ?? string.Empty),
                    new Claim(ClaimTypes.Email, pharmacy.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role,  "Pharmacy"),
                    new Claim("PharmacyId", pharmacy.PharmId.ToString())
                };

                var pharmacyIdentity = new ClaimsIdentity(
                    pharmacyClaims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var pharmacyPrincipal = new ClaimsPrincipal(pharmacyIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    pharmacyPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(8)
                    });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                // صيدلي → Home فيها Pharmacy Dashboard
                return RedirectToAction("Index", "Home");
            }

            // 2) لو مش صيدلية، نجرب كـ Customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null)
            {
                ViewBag.Error = "No account found with this email.";
                return View(model);
            }

            var customerPassword = customer.PasswordHash?.Trim(); // حسب اسم العمود عندك

            if (!string.Equals(customerPassword, password, StringComparison.Ordinal))
            {
                ViewBag.Error = "Wrong password.";
                return View(model);
            }

            var customerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  customer.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, customer.Email ?? string.Empty),
                new Claim(ClaimTypes.Role,  "Customer"),
                new Claim("CustomerId", customer.CustomerId.ToString())
            };

            var customerIdentity = new ClaimsIdentity(
                customerClaims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var customerPrincipal = new ClaimsPrincipal(customerIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                customerPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // كاستمر → Home العادية
            return RedirectToAction("Index", "Home");
        }

        // =======================
        //     REGISTER (GET)
        // =======================
        [HttpGet]
        public IActionResult Register()
        {
            return View(new CustomerRegisterViewModel());
        }

        // =======================
        //     REGISTER (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _context.Customers
                .AnyAsync(c => c.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
                return View(model);
            }

            var customer = new Customer
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Email = model.Email,
                // مؤقتاً بدون Hash – بعدين تعملي Hash
                PasswordHash = model.Password
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Login تلقائي بعد التسجيل
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  customer.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, customer.Email ?? string.Empty),
                new Claim(ClaimTypes.Role,  "Customer"),
                new Claim("CustomerId", customer.CustomerId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });

            return RedirectToAction("Index", "Home");
        }

        // =======================
        //        LOGOUT
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
