using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ChatPageController : Controller
    {
        public IActionResult Index()
        {
            return View("Chat");
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
