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
}
