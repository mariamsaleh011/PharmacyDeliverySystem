
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
using System;
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

        private int GetPharmacyId() => 1; // مؤقت للتجربة

        // List all chats for this pharmacy
        public IActionResult Chats()
        {
            int pharmacyId = GetPharmacyId();

            var chats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == pharmacyId)
                .Select(c => new PharmacyChatListItemViewModel
                {
                    ChatId = c.ChatId,
                    CustomerName = c.Customer.Name,
                    LastMessage = c.ChatMessages
                                   .OrderByDescending(m => m.SentAt)
                                   .FirstOrDefault().MessageText,
                    LastMessageTime = c.ChatMessages
                                   .OrderByDescending(m => m.SentAt)
                                   .FirstOrDefault().SentAt
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return View(chats);
        }

        // Open chat with a customer
        public IActionResult PhChat(int id)
        {
            int pharmacyId = GetPharmacyId();

            var chat = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == id && c.PharmacyId == pharmacyId);

            if (chat == null) return NotFound();

            return View(chat);
        }

        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("PhChat", new { id = chatId });

            var chat = _context.Chats.Find(chatId);
            if (chat == null) return NotFound();

            _context.ChatMessages.Add(new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Pharmacy",
                MessageText = message,
                SentAt = DateTime.Now
            });
            _context.SaveChanges();

            return RedirectToAction("PhChat", new { id = chatId });
        }
    }
}


