using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers;

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
        // جلب المنتجات اللي هتظهر كعروض في الهوم
        var offersProducts = _productManager.GetAll();

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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction("Index");

        var allProducts = _productManager.GetAll();  // ← بيجيب كل المنتجات

        var results = allProducts
                        .Where(p => p.Name.StartsWith(query) ||
                                    p.Description.StartsWith(query) ||
                                    p.DrugType.StartsWith(query)) // لو عايزة كمان يبحث بالكاتيجوري
                        .ToList();

        return View("SearchResults", results);
    }

}
