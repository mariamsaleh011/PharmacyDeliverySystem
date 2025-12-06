using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
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

        public HomeController(ILogger<HomeController> logger, IProductManager productManager)
        {
            _logger = logger;
            _productManager = productManager;
        }

        // =============================
        // الصفحة الرئيسية
        // =============================
        public IActionResult Index()
        {
            // نجيب كل المنتجات مرة واحدة
            var allProducts = _productManager.GetAll().ToList();

            // المنتجات اللي عليها خصم فقط (للأوفرز)
            var offersProducts = allProducts
                                 .Where(p => p.OldPrice.HasValue &&
                                             p.OldPrice.Value > p.Price)
                                 .ToList();

            ViewBag.OffersProducts = offersProducts;   // للأوفرز فقط

            // أعلى المنتجات مبيعًا (هنا بنختار 4 برودكت مميزة)
            // تقدر لاحقًا تبدّل الترتيب ده بترتيب حسب المبيعات الفعلية
            var topSellingProducts = allProducts
                                     .OrderByDescending(p => p.ProId) // مؤقتًا بالـ Id
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

        public IActionResult ChatRedirect()
        {
            // لو مش عامل Login أصلاً
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                // نودّيه على صفحة اللوجين بتاعة CustomerAuth
                return RedirectToAction("Login", "CustomerAuth");
            }

            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim != null)
            {
                // لو العميل
                if (roleClaim.Value == "Customer")
                {
                    return RedirectToAction("Index", "Chat", new { pharmacyId = 1 });
                }

                // لو الصيدلي
                if (roleClaim.Value == "Pharmacy")
                {
                    return RedirectToAction("Chats", "PharmacyChat");
                }
            }

            // في حالة فشل تحديد الدور
            return RedirectToAction("AccessDenied", "Account");
        }

        public IActionResult GoToChat()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == "Customer")
                    return RedirectToAction("Index", "Chat"); // صفحة الدردشة للعميل

                if (role == "Pharmacy")
                    return RedirectToAction("Chats", "PharmacyChat"); // صفحة الدردشة للصيدلي
            }

            // إذا لم يكن مسجل دخول
            return RedirectToAction("Login", "CustomerAuth");
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Cart()
        {
            // هنا عايزين كل المنتجات علشان تظهر في العمود الشمال
            var products = _productManager.GetAll().ToList();
            return View(products);
        }
    }
}
