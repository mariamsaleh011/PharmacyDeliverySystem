using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Controllers
{
    public class DrugsController : Controller
    {
        public IActionResult Index()
        {
            return View("Drugs");
        }
    }
}
