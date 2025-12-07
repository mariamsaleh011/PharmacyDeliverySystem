using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace PharmacyDeliverySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductManager _productManager;
        private readonly PharmacyDeliveryContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            IProductManager productManager,
            PharmacyDeliveryContext context)
        {
            _logger = logger;
            _productManager = productManager;
            _context = context;
        }

        // =============================
        // الصفحة الرئيسية
        // =============================
        public IActionResult Index()
        {
            var allProducts = _productManager.GetAll().ToList();

            var offersProducts = allProducts
                                 .Where(p => p.OldPrice.HasValue &&
                                             p.OldPrice.Value > p.Price)
                                 .ToList();

            ViewBag.OffersProducts = offersProducts;

            var topSellingProducts = allProducts
                                     .OrderByDescending(p => p.ProId)
                                     .Take(4)
                                     .ToList();

            ViewBag.TopSellingProducts = topSellingProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            var lowerQuery = query.ToLower();

            var results = _productManager.GetAll()
                         .Where(p =>
                                (p.Name ?? string.Empty).ToLower().Contains(lowerQuery) ||
                                (!string.IsNullOrEmpty(p.Description) &&
                                    p.Description!.ToLower().Contains(lowerQuery)) ||
                                (p.DrugType ?? string.Empty).ToLower().Contains(lowerQuery))
                         .ToList();

            return View("SearchResults", results);
        }

        // =============================
        //  ChatRedirect من النافبار
        // =============================
        public IActionResult ChatRedirect()
        {
            // لو مش عامل Login أصلاً
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "CustomerAuth");
            }

            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // ---- لو كاستمر: نختار صيدلية من الـ DB ونعمل شات معاها ----
            if (role == "Customer")
            {
                // هنا بنختار أول صيدلية في الجدول (ممكن بعدين تعملي منيو اختيار)
                var defaultPharmacyId = _context.Pharmacies
                                                .Select(p => p.PharmId)
                                                .OrderBy(id => id)
                                                .FirstOrDefault();

                if (defaultPharmacyId == 0)
                {
                    // مفيش صيدليات في الداتا بيز
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "Chat", new { pharmacyId = defaultPharmacyId });
            }

            // ---- لو صيدلي: يروح على صفحة الشات بتاعة الصيدلي ----
            if (role == "Pharmacy")
            {
                return RedirectToAction("Chats", "PharmacyChat");
            }

            // لو دور غريب، رجّعيه للهوم
            return RedirectToAction("Index");
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Cart()
        {
            var products = _productManager.GetAll().ToList();
            return View(products);
        }
    }
}
