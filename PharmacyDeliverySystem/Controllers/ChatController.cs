
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ChatController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public ChatController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // Helper to get logged-in CustomerId
        private int GetCustomerId()
        {
            return int.Parse(User.Claims.First(c => c.Type == "CustomerId").Value);
        }

        // GET: /Chat
        public IActionResult Index(int pharmacyId = 1) // default to pharmacy 1
        {
            int customerId = GetCustomerId();

            var chat = _context.Chats
                .Include(c => c.Pharmacy)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.PharmacyId == pharmacyId && c.CustomerId == customerId);

            // If no chat exists, create one
            if (chat == null)
            {
                chat = new Chat
                {
                    PharmacyId = pharmacyId,
                    CustomerId = customerId,
                    Status = "Open",
                    Channel = "Default"
                };

                _context.Chats.Add(chat);
                _context.SaveChanges();

                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .FirstOrDefault(c => c.ChatId == chat.ChatId);
            }

            return View("Index", chat); // explicitly use Index.cshtml
        }

        // POST: /Chat/SendMessage
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Index");

            var chat = _context.Chats.FirstOrDefault(c => c.ChatId == chatId);
            if (chat == null)
                return NotFound();

            var newMessage = new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Customer",
                MessageText = message,
                SentAt = System.DateTime.Now
            };

            _context.ChatMessages.Add(newMessage);
            _context.SaveChanges();

            return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
        }
    }
}

