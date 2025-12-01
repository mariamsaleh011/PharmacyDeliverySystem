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

            // 1) جرّب كـ Customer
            var customer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);

            if (customer != null && customer.PasswordHash == model.Password)
            {
                await SignInUser(customer.Name, customer.Email, "Customer");
                return RedirectAfterLogin(returnUrl, "Customer");
            }

            // 2) لو مش Customer.. جرّب كـ Pharmacy
            var pharmacy = _context.Pharmacies.FirstOrDefault(p => p.Email == model.Email);

            if (pharmacy != null && pharmacy.PasswordHash == model.Password)
            {
                await SignInUser(pharmacy.Name, pharmacy.Email, "Pharmacy");
                return RedirectAfterLogin(returnUrl, "Pharmacy");
            }

            // 3) مفيش حد ماتش
            ViewBag.Error = "Invalid email or password";
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // helper: يعمل SignIn ويحط الـ Role
        private async Task SignInUser(string? name, string? email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  name  ?? string.Empty),
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Role,  role) // "Customer" أو "Pharmacy"
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }

        // helper: يحدد يروح فين بعد الـ Login على حسب الـ Role
        private IActionResult RedirectAfterLogin(string? returnUrl, string role)
        {
            // لو كان داخل على صفحة محتاجة Login نرجعه لها
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // الوجهة الافتراضية لكل Role
            return role switch
            {
                "Customer" => RedirectToAction("Index", "Home"),
                "Pharmacy" => RedirectToAction("Admin", "Product"), // 👈 الصيدلي يروح للأدمن
                _ => RedirectToAction("Index", "Home")
            };
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

            // تأكد إن الإيميل مش مكرر في الكاستمر
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
                PasswordHash = model.Password   // مؤقتاً من غير Hash
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Login تلقائي بعد التسجيل كـ Customer
            await SignInUser(customer.Name, customer.Email, "Customer");

            return RedirectToAction("Index", "Home");
        }

        // =======================
        //        LOGOUT
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
