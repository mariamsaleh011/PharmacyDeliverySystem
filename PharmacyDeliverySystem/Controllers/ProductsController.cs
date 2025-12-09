using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public ProductsController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // ===== قائمة كل المنتجات =====
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            ViewBag.ProductsList = products;
            return View();
        }

        // ===== صفحة تفاصيل منتج واحد =====
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
