using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
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

        // ============================
        // فتح الشات للعميل
        // ============================
        public IActionResult Index(int? pharmacyId)
        {
            int customerId = GetCustomerId();
            Chat? chat;

            if (pharmacyId.HasValue)
            {
                // حالة خاصة لو حبيتي تربطي الشات بصيدلية معيّنة
                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .FirstOrDefault(c =>
                        c.CustomerId == customerId &&
                        c.PharmacyId == pharmacyId.Value);

                if (chat == null)
                {
                    chat = new Chat
                    {
                        PharmacyId = pharmacyId.Value,
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
            }
            else
            {
                // مود الـ Inbox المشتركة:
                // هات آخر شات Open للـ Customer
                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .Where(c => c.CustomerId == customerId && c.Status == "Open")
                    .OrderByDescending(c => c.ChatId)
                    .FirstOrDefault();

                // لو مفيش شات → نعمل واحد جديد بـ CustomerId بس، و PharmacyId = null
                if (chat == null)
                {
                    chat = new Chat
                    {
                        CustomerId = customerId,
                        Status = "Open",
                        Channel = "Default",
                        PharmacyId = null
                    };

                    _context.Chats.Add(chat);
                    _context.SaveChanges();

                    chat = _context.Chats
                        .Include(c => c.Pharmacy)
                        .Include(c => c.ChatMessages)
                        .FirstOrDefault(c => c.ChatId == chat.ChatId);
                }
            }

            return View(chat);
        }

        // ============================
        // إرسال رسالة من العميل
        // ============================
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message, IFormFile? file)
        {
            var chat = _context.Chats.Find(chatId);
            if (chat == null) return NotFound();

            if (string.IsNullOrWhiteSpace(message) &&
                (file == null || file.Length == 0))
            {
                return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
            }

            // لو عايزة بعدين تحفظي الروشتة في ملف
            // Placeholder للكود

            _context.ChatMessages.Add(new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Customer",
                MessageText = message,
                SentAt = DateTime.Now
            });

            _context.SaveChanges();

            // لو PharmacyId = null → هيرجع لـ Index عادي
            return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
        }
    }
}
