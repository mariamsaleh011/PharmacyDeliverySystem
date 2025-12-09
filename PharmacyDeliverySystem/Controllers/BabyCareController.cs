using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize]
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
        [AllowAnonymous] // خطأ السيرفر ممكن يظهر لأي حد
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
