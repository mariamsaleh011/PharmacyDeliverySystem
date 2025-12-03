using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

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

        public IActionResult Index()
        {
            // عرض المنتجات اللي عليها خصم فقط
            var offersProducts = _productManager.GetAll()
                                 .Where(p => p.OldPrice.HasValue &&
                                             p.OldPrice.Value > p.Price)
                                 .ToList();

            ViewBag.OffersProducts = offersProducts;

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
                return RedirectToAction("Login", "CustomerAccount");
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
            if (User.Identity.IsAuthenticated)
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

    }
}
