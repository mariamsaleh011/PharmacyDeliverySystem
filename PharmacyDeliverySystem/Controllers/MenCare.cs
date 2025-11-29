using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Controllers
{
    public class MenCareController : Controller
    {
        public IActionResult Index()
        {
            // Views/MenCare/MenCare.cshtml
            return View("MenCare");
        }
    }
}
