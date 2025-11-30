using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.ViewModels.Chat;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ChatController : Controller
    {
        private readonly IChatManager _chatManager;

        public ChatController(IChatManager chatManager)
        {
            _chatManager = chatManager;
        }

        // GET: /Chat
        public IActionResult Index()
        {
            // هيستخدم Views/Chat/Chat.cshtml لأننا سميناه "Chat"
            return View("Chat");
        }

        // لو عايزة /Chat/Chat
        public IActionResult Chat()
        {
            return View("Chat");
        }
    }
}
