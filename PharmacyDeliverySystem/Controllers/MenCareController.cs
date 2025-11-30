using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers;

public class MenCareController : Controller
{
    private readonly ILogger<MenCareController> _logger;
    private readonly IProductManager _productManager;

    public MenCareController(ILogger<MenCareController> logger, IProductManager productManager)
    {
        _logger = logger;
        _productManager = productManager;
    }

    // صفحة Men Care
    public IActionResult Index()
    {
        // جلب المنتجات الخاصة بـ Men Care
        var menCareProducts = _productManager.GetAll()
                                 .Where(p => p.DrugType == "men care")
                                 .ToList();

        ViewBag.MenCareProducts = menCareProducts;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
