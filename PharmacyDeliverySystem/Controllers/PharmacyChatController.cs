
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyChatController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyChatController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        private int GetPharmacyId() => int.Parse(User.Claims.First(c => c.Type == "PharmacyId").Value);

        // List all chats
        public IActionResult Chats()
        {
            int pharmacyId = GetPharmacyId();

            var chats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == pharmacyId)
                .Select(c => new
                {
                    c.ChatId,
                    CustomerName = c.Customer.Name,
                    LastMessage = c.ChatMessages.OrderByDescending(m => m.SentAt).FirstOrDefault().MessageText,
                    LastMessageTime = c.ChatMessages.OrderByDescending(m => m.SentAt).FirstOrDefault().SentAt
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return View(chats);
        }

        // Open chat with a customer
        public IActionResult Chat(int id)
        {
            int pharmacyId = GetPharmacyId();

            var chat = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == id && c.PharmacyId == pharmacyId);

            if (chat == null)
                return NotFound();

            return View(chat);
        }

        // Send message
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Chat", new { id = chatId });

            var chat = _context.Chats.FirstOrDefault(c => c.ChatId == chatId);
            if (chat == null)
                return NotFound();

            var newMessage = new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Pharmacy",
                MessageText = message,
                SentAt = System.DateTime.Now
            };

            _context.ChatMessages.Add(newMessage);
            _context.SaveChanges();

            return RedirectToAction("Chat", new { id = chatId });
        }
    }
}


