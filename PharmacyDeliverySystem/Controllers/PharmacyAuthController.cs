using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /* ==================== LOGOUT ==================== */

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // بعد ما يسجّل خروج، يرجع لصفحة اللوجين الموحدة
            return RedirectToAction("Login", "CustomerAuth");
        }

        /* ==================== REGISTER (GET) ==================== */

        [HttpGet]
        public IActionResult Register()
        {
            return View(new PharmacyRegisterViewModel());
        }

        /* ==================== REGISTER (POST) ==================== */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(PharmacyRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = _context.Pharmacies.Any(p => p.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var pharmacy = new Pharmacy
            {
                Name = model.Name,
                Email = model.Email,
                // مؤقتاً من غير Hash
                PasswordHash = model.Password
            };

            _context.Pharmacies.Add(pharmacy);
            _context.SaveChanges();

            // بعد ما يعمل Sign up كصيدلية → يروح للوجين الموحد
            return RedirectToAction("Login", "CustomerAuth");
        }
    }
}
