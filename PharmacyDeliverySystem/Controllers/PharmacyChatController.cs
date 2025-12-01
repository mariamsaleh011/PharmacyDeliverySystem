using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    public class PharmacyChatController : Controller
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyChatController(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // ✅ جلب PharmacyId من الـ Session بعد تسجيل الدخول
        private int GetCurrentPharmacyId()
        {
            var id = HttpContext.Session.GetInt32("PharmacyId");
            if (id.HasValue)
                return id.Value;
            else
                throw new Exception("Pharmacy is not logged in.");
        }

        // 1️⃣ عرض كل الشاتس الخاصة بالصيدلية
        public IActionResult Index()
        {
            int pharmacyId = GetCurrentPharmacyId();

            var chats = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.Order)
                .Include(c => c.ChatMessages)
                .Where(c => c.PharmacyId == pharmacyId)
                .OrderByDescending(c => c.ChatId)
                .ToList();

            return View(chats);
        }

        // 2️⃣ فتح شات معين
        public IActionResult Chat(int id)
        {
            var chat = _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == id && c.PharmacyId == GetCurrentPharmacyId());

            if (chat == null)
                return NotFound();

            return View(chat);
        }

        // 3️⃣ إرسال رسالة من الصيدلي
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Chat", new { id = chatId });

            var chat = _context.Chats
                .FirstOrDefault(c => c.ChatId == chatId && c.PharmacyId == GetCurrentPharmacyId());

            if (chat == null)
                return NotFound();

            var newMessage = new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Pharmacy",
                MessageText = message,
                SentAt = DateTime.Now
            };

            _context.ChatMessages.Add(newMessage);
            _context.SaveChanges();

            return RedirectToAction("Chat", new { id = chatId });
        }
    }
}

