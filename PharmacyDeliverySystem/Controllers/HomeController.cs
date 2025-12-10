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
                .Where(p => p.OldPrice.HasValue && p.OldPrice.Value > p.Price)
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

                // ===============================
                // 🔔 عدد الشاتات اللي فيها رسائل جديدة للـ Pharmacy الحالية بس
                // ===============================
                int? pharmacyId = null;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrWhiteSpace(email))
                {
                    pharmacyId = _context.Pharmacies
                        .Where(p => p.Email == email)
                        .Select(p => (int?)p.PharmId)
                        .FirstOrDefault();
                }

                int newChatsCount;

                if (pharmacyId.HasValue)
                {
                    newChatsCount = _context.Chats
                        .Include(c => c.ChatMessages)
                        .Where(c =>
                            c.Status == "Open" &&
                            (c.PharmacyId == null || c.PharmacyId == pharmacyId.Value))
                        .Count(c => c.ChatMessages
                            .Any(m => m.SenderType == "Customer" && !m.IsRead));
                }
                else
                {
                    newChatsCount = _context.Chats
                        .Include(c => c.ChatMessages)
                        .Where(c => c.Status == "Open" && c.PharmacyId == null)
                        .Count(c => c.ChatMessages
                            .Any(m => m.SenderType == "Customer" && !m.IsRead));
                }

                ViewBag.NewChatsCount = newChatsCount;
            }

            // مهم ترجع View في كل الحالات (حتى لو مش Pharmacy)
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

        // =============================
        // صفحة نتائج السيرش الكلاسيكية
        // =============================
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
        // 🔎 API للسيرش الخاص بالهيدر (JSON)
        // يتنادى من الـ JavaScript في أي صفحة
        // =============================
        [HttpGet]
        public IActionResult SearchJson(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(Enumerable.Empty<object>());
            }

            var lowerQuery = query.ToLower();

            var results = _productManager.GetAll()
                .Where(p =>
                    (p.Name ?? string.Empty).ToLower().Contains(lowerQuery) ||
                    (!string.IsNullOrEmpty(p.Description) &&
                        p.Description!.ToLower().Contains(lowerQuery)) ||
                    (p.DrugType ?? string.Empty).ToLower().Contains(lowerQuery))
                .Select(p => new
                {
                    id = p.ProId,
                    name = p.Name ?? string.Empty,
                    description = (p.Description ?? p.Dosage) ?? string.Empty,
                    price = p.Price,
                    oldPrice = p.OldPrice,
                    imageUrl = string.IsNullOrWhiteSpace(p.ImageUrl)
                        ? Url.Content("~/images/icons/product-default.svg")
                        : p.ImageUrl,
                    detailsUrl = Url.Action("Details", "Products", new { id = p.ProId })
                })
                .Take(10)
                .ToList();

            return Json(results);
        }

        // =============================
        //  ChatRedirect من النافبار
        // =============================
        public IActionResult ChatRedirect()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "CustomerAuth");
            }

            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == "Customer")
            {
                return RedirectToAction("Index", "Chat");
            }

            if (role == "Pharmacy")
            {
                return RedirectToAction("Chats", "PharmacyChat");
            }

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
