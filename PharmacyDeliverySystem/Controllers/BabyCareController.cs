using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    public class BabyCareController : Controller
    {
        private readonly ILogger<BabyCareController> _logger;
        private readonly IProductManager _productManager;

        public BabyCareController(ILogger<BabyCareController> logger, IProductManager productManager)
        {
            _logger = logger;
            _productManager = productManager;
        }

        public IActionResult Index()
        {
            var babyCareProducts = _productManager.GetAll()
                                  .Where(p => p.DrugType == "baby care")
                                  .ToList();

            ViewBag.BabyCareProducts = babyCareProducts;
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
    }
}
