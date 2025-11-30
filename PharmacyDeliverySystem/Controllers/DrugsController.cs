using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    public class DrugsController : Controller
    {
        private readonly ILogger<DrugsController> _logger;
        private readonly IProductManager _productManager;

        public DrugsController(ILogger<DrugsController> logger, IProductManager productManager)
        {
            _logger = logger;
            _productManager = productManager;
        }

        public IActionResult Index()
        {
            var drugsProducts = _productManager.GetAll()
                                .Where(p => p.DrugType == "drugs")
                                .ToList();

            ViewBag.DrugsProducts = drugsProducts;
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
