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

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            ViewBag.ProductsList = products;
            return View();
        }

    }
}
