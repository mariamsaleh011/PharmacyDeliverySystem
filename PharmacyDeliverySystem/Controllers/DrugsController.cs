using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
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
                                .Where(p => p.DrugType == "Drugs")
                                .ToList();

            ViewBag.DrugsProducts = drugsProducts;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
