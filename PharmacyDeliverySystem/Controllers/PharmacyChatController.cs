using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;

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

        /// <summary>
        /// يرجّع رقم الصيدلية اعتمادًا على الإيميل الموجود في الـ Claims
        /// </summary>
        private int? GetPharmacyId()
        {
            // 1) نجيب الإيميل من الكليمز
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
                return null;

            // 2) نجيب الصيدلية اللي إيميلها ده
            var pharmacy = _context.Pharmacies
                                   .FirstOrDefault(p => p.Email == email);

            return pharmacy?.PharmId;
        }

        // =======================
        //  قائمة الشاتات للصيدلي
        // =======================
        public IActionResult Chats()
        {
            var pharmacyId = GetPharmacyId();
            if (pharmacyId == null)
            {
                // لو لأي سبب مش لاقيين صيدلية مرتبطة باليوزر → رجعه للّوجين
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

        // =======================
        //  فتح شات معين
        // =======================
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

        // =======================
        //  إرسال رسالة من الصيدلي
        // =======================
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

            // تأكيد إن الشات تابع للصيدلي ده فعلاً
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
