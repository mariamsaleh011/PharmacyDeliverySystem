using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Controllers
{
    public class BabyCareController : Controller
    {
        public IActionResult Index()
        {
            return View("babycare");
        }
    }
}
