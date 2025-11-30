using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Linq;

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

            var results = _productManager.GetAll()
                         .Where(p =>
                                p.Name.ToLower().Contains(query.ToLower()) ||
                                (p.Description != null && p.Description.ToLower().Contains(query.ToLower())) ||
                                p.DrugType.ToLower().Contains(query.ToLower()))
                         .ToList();

            return View("SearchResults", results);
        }
    }
}
