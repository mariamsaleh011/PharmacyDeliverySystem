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

        private int? GetPharmacyId()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return null;

            var ph = _context.Pharmacies.FirstOrDefault(p => p.Email == email);
            return ph?.PharmId;
        }

        public IActionResult Chats()
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
                return RedirectToAction("Login", "PharmacyAuth");

            var newChats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == null && c.Status == "Open")
                .Select(c => new PharmacyChatListItemViewModel
                {
                    ChatId = c.ChatId,
                    CustomerName = c.Customer != null ? c.Customer.Name : "Unknown customer",
                    LastMessage = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.MessageText)
                        .FirstOrDefault() ?? "",
                    LastMessageTime = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault(),
                    IsNew = true
                })
                .ToList();

            var myChats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == pharmacyId && c.Status == "Open")
                .Select(c => new PharmacyChatListItemViewModel
                {
                    ChatId = c.ChatId,
                    CustomerName = c.Customer != null ? c.Customer.Name : "Unknown customer",
                    LastMessage = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.MessageText)
                        .FirstOrDefault() ?? "",
                    LastMessageTime = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault(),
                    IsNew = false
                })
                .ToList();

            var vm = new PharmacyChatsPageViewModel
            {
                NewChats = newChats,
                MyChats = myChats
            };

            return View(vm);
        }

        public IActionResult PhChat(int id)
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
                return RedirectToAction("Login", "PharmacyAuth");

            var chat = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == id);

            if (chat == null)
                return NotFound();

            if (chat.PharmacyId == null)
            {
                chat.PharmacyId = pharmacyId.Value;
                _context.SaveChanges();
            }

            return View(chat);
        }

        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
                return RedirectToAction("Login", "PharmacyAuth");

            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("PhChat", new { id = chatId });

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
