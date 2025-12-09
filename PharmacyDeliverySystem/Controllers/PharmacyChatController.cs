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

        // ======================= قائمة الشاتات للصيدلي =======================
        public IActionResult Chats()
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
                return RedirectToAction("Login", "PharmacyAuth");

            // 🆕 شاتات لسه محدش استلمها (PharmacyId = null)
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

                    // عدد الرسائل من الكاستمر اللي لسه متقريتش
                    UnreadCount = c.ChatMessages
                        .Count(m => m.SenderType == "Customer" && !m.IsRead),

                    // الشات جديد لو فيه أي رسالة من الكاستمر IsRead = false
                    IsNew = c.ChatMessages
                        .Any(m => m.SenderType == "Customer" && !m.IsRead)
                })
                .ToList();

            // 🧑‍⚕️ الشاتات اللي الصيدلية دي استلمتها
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

                    UnreadCount = c.ChatMessages
                        .Count(m => m.SenderType == "Customer" && !m.IsRead),

                    IsNew = c.ChatMessages
                        .Any(m => m.SenderType == "Customer" && !m.IsRead)
                })
                .ToList();

            var vm = new PharmacyChatsPageViewModel
            {
                NewChats = newChats,
                MyChats = myChats
            };

            return View(vm);
        }

        // ======================= صفحة شات واحدة للصيدلي =======================
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

            // أول ما الصيدلي يفتح الشات يمسكه لو كان لسه مش متحدد
            if (chat.PharmacyId == null)
            {
                chat.PharmacyId = pharmacyId.Value;
            }

            // علّم كل رسائل الكاستمر اللي لسه متقريتش إنها اتقرت
            var unreadFromCustomer = chat.ChatMessages
                .Where(m => m.SenderType == "Customer" && !m.IsRead)
                .ToList();

            if (unreadFromCustomer.Any())
            {
                foreach (var m in unreadFromCustomer)
                {
                    m.IsRead = true;
                }
            }

            _context.SaveChanges();

            return View(chat);
        }

        // ======================= إرسال رسالة من الصيدلي =======================
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
                return RedirectToAction("Login", "PharmacyAuth");

            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("PhChat", new { id = chatId });

            var chat = _context.Chats
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == chatId);

            if (chat == null)
                return NotFound();

            var msg = new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Pharmacy",
                MessageText = message,
                SentAt = DateTime.Now,
                IsRead = true   // رسالة الصيدلي نفسه تعتبر مقروءة
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            return RedirectToAction("PhChat", new { id = chatId });
        }
    }
}
