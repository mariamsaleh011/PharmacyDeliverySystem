using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.ViewModels.Chat;


namespace PharmacyDeliverySystem.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PharmacyDeliverySystem.Models;
    using PharmacyDeliverySystem.Business.Interfaces;
    using PharmacyDeliverySystem.ViewModels.Chat;

    namespace PharmacyDeliverySystem.Controllers
    {
        [Authorize(Roles = "customer")]
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

            return View();
        }
    }
}
