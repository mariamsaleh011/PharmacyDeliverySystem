
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

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

        private int GetCustomerId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
            if (claim == null) throw new Exception("CustomerId claim not found.");
            return int.Parse(claim.Value);
        }

        // Open chat with a pharmacy
        public IActionResult Index(int pharmacyId = 1)
        {
            int customerId = GetCustomerId();

            var chat = _context.Chats
                .Include(c => c.Pharmacy)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.PharmacyId == pharmacyId && c.CustomerId == customerId);

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

            return View(chat);
        }

        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Index");

            var chat = _context.Chats.Find(chatId);
            if (chat == null) return NotFound();

            _context.ChatMessages.Add(new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Customer",
                MessageText = message,
                SentAt = DateTime.Now
            });

            _context.SaveChanges();

            return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
        }
    }
}
