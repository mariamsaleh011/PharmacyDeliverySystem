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

        // Open chat (specific pharmacy لو جايه, أو آخر شات للمستخدم لو لأ)
        public IActionResult Index(int? pharmacyId)
        {
            int customerId = GetCustomerId();
            Chat? chat;

            if (pharmacyId.HasValue)
            {
                // شات مع الصيدلية اللي جايه في الباراميتر
                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .FirstOrDefault(c =>
                        c.CustomerId == customerId &&
                        c.PharmacyId == pharmacyId.Value);

                // لو مفيش شات قبل كده بين الكاستمر والصيدلية دي -> نعمل واحد جديد
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
                // مفيش pharmacyId: هات آخر شات للـ Customer وافتحه
                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .Where(c => c.CustomerId == customerId)
                    .OrderByDescending(c => c.ChatId)   // الأحدث
                    .FirstOrDefault();

                if (chat == null)
                {
                    // مفيش أي شات لسه -> رجّعه للهوم (أو لاحقًا صفحة اختيار صيدلية)
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(chat);
        }

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

            // TODO: حفظ ملف الروشتة في uploads وتخزين الـ Path في ChatMessage

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
