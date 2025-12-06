using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
//using PharmacyDeliverySystem.ViewModels.PharmacyChat;
using System;
using System.Linq;
using System.Security.Claims;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyChatController : Controller
    {
        private readonly PharmacyDeliveryContext _context;
        private readonly IPharmacyManager _pharmacyManager;

        public PharmacyChatController(
            PharmacyDeliveryContext context,
            IPharmacyManager pharmacyManager)
        {
            _context = context;
            _pharmacyManager = pharmacyManager;
        }

        /// <summary>
        /// يرجّع رقم الصيدلية من الـ Claims (من خلال الإيميل)
        /// </summary>
        /// <returns>PharmId أو null لو مش موجود</returns>
        private int? GetPharmacyId()
        {
            // جرّب الأول Name، ولو فاضية جرّب Email
            var email =
                User.FindFirst(ClaimTypes.Name)?.Value ??
                User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return null;

            var pharmacy = _pharmacyManager.GetPharmacyByEmail(email);
            return pharmacy?.PharmId;
        }

        // List all chats for this pharmacy
        public IActionResult Chats()
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
            {
                // مش لاقي صيدلية مرتبطة باليوزر → رجعه لصفحة لوجين الصيدلي مثلاً
                return RedirectToAction("Login", "PharmacyAuth");
            }

            var chats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == pharmacyId.Value)
                .Select(c => new PharmacyChatListItemViewModel
                {
                    ChatId = c.ChatId,

                    CustomerName = c.Customer != null
                        ? c.Customer.Name
                        : "Unknown customer",

                    LastMessage = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.MessageText)
                        .FirstOrDefault() ?? string.Empty,

                    LastMessageTime = c.ChatMessages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault()
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();

            return View(chats);
        }

        // Open chat with a customer
        public IActionResult PhChat(int id)
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
            {
                return RedirectToAction("Login", "PharmacyAuth");
            }

            var chat = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == id && c.PharmacyId == pharmacyId.Value);

            if (chat == null) return NotFound();

            return View(chat);
        }

        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
            {
                return RedirectToAction("Login", "PharmacyAuth");
            }

            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("PhChat", new { id = chatId });

            // نتأكد إن الشات فعلاً تابع للصيدلية دي
            var chat = _context.Chats
                .FirstOrDefault(c => c.ChatId == chatId && c.PharmacyId == pharmacyId.Value);

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
