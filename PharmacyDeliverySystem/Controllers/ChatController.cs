using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]   // ممكن على مستوى الكنترولر كله
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View("Chat");   // أو View(); لو اسم الفيو Index
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
