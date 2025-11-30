using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
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
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);

            // مقارنة الباسورد مع PasswordHash اللي في الموديل
            if (customer == null || customer.PasswordHash != model.Password)
            {
                ViewBag.Error = "Invalid email or password";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

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

            // تأكد إن الإيميل مش مكرر
            bool emailExists = _context.Customers.Any(c => c.Email == model.Email);
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
                // نخزن الباسورد في PasswordHash
                PasswordHash = model.Password      // (مفيش hashing دلوقتي)
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Login تلقائي بعد التسجيل
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // =======================
        //        LOGOUT
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
