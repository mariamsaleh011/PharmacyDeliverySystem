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
    public class OffersController : Controller
    {
        private readonly ILogger<OffersController> _logger;
        private readonly IProductManager _productManager;

        public OffersController(ILogger<OffersController> logger, IProductManager productManager)
        {
            _logger = logger;
            _productManager = productManager;
        }

        public IActionResult Index()
        {
            // عدّل الفلتر لو عندكم Flag تاني للأوفرز
            var offers = _productManager.GetAll()
                            .Where(p => p.OldPrice.HasValue && p.OldPrice.Value > p.Price)
                            .ToList();

            ViewBag.OffersProducts = offers;
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
