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
            // ===== منتجات الموقع (Offers + TopSelling) =====
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

            // ===== أرقام الداشبورد للفارمسي فقط =====
            if (User.Identity != null &&
                User.Identity.IsAuthenticated &&
                User.IsInRole("Pharmacy"))
            {
                // إجمالي الأوردرات
                ViewBag.TotalOrders = _context.Orders.Count();

                // الأوردرات الجديدة (مثلاً Pending)
                ViewBag.NewOrdersCount = _context.Orders
                    .Count(o => o.Status == "Pending");

                // طلبات الـ Return اللي لسه مستنية قرار
                ViewBag.PendingReturnsCount = _context.Returns
                    .Count(r => r.Status == "Pending" || r.Status == "Requested");
            }

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

            // لو كاستمر → يروح على صفحة الشات العامة من غير PharmacyId
            if (role == "Customer")
            {
                return RedirectToAction("Index", "Chat");
            }

            // لو صيدلي → يروح على صفحة الشاتات بتاعة الصيدلي
            if (role == "Pharmacy")
            {
                return RedirectToAction("Chats", "PharmacyChat");
            }

            // أي دور غريب → رجّعه للهوم
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
