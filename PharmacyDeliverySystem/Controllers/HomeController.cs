using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using System.Collections.Generic;
using PharmacyDeliverySystem.Models;
using System.Diagnostics;

namespace PharmacyDeliverySystem.Controllers;

public class HomeController : Controller
{
    private readonly IProductManager _productManager;
    public HomeController(IProductManager productManager)
    {
        _productManager = productManager;
    }

    public IActionResult Index()
    {
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