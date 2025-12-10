using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Models.Validation;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ChatController : Controller
    {
        private readonly PharmacyDeliveryContext _context;
        private readonly IWebHostEnvironment _env;

        public ChatController(PharmacyDeliveryContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int GetCustomerId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
            if (claim == null) throw new Exception("CustomerId claim not found.");
            return int.Parse(claim.Value);
        }

        // ============================ فتح الشات ============================
        public IActionResult Index(int? pharmacyId)
        {
            int customerId = GetCustomerId();
            Chat? chat;

            if (pharmacyId.HasValue)
            {
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
                chat = _context.Chats
                    .Include(c => c.Pharmacy)
                    .Include(c => c.ChatMessages)
                    .Where(c => c.CustomerId == customerId && c.Status == "Open")
                    .OrderByDescending(c => c.ChatId)
                    .FirstOrDefault();

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

            // جلب آخر روشتة محفوظة
            Prescription? lastPrescription = null;

            if (chat.PharmacyId.HasValue)
            {
                lastPrescription = _context.Prescriptions
                    .Where(p => p.CustomerId == customerId && p.PharmId == chat.PharmacyId.Value)
                    .OrderByDescending(p => p.PreId)
                    .FirstOrDefault();
            }

            ViewBag.LastPrescription = lastPrescription;

            return View(chat);
        }

        // ======================= إرسال رسالة من العميل =======================
        [HttpPost]
        public IActionResult SendMessage(int chatId, string? message, IFormFile? file)
        {
            var chat = _context.Chats.Find(chatId);
            if (chat == null) return NotFound();

            if (string.IsNullOrWhiteSpace(message) && (file == null || file.Length == 0))
            {
                return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
            }

            // --- حفظ الفايل ---
            if (file != null && file.Length > 0)
            {
                // ✅ مسموح صور + PDF فقط
                var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();

                var allowedExtensions = new[]
                {
                    ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".pdf"
                };

                // لو نوع الملف مش من الليستة → رجّع Error وماتكملش
                if (string.IsNullOrWhiteSpace(ext) || !allowedExtensions.Contains(ext))
                {
                    TempData["UploadError"] = "يمكنك رفع صور أو ملفات PDF فقط (روشتة).";
                    return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "images", "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // debug log (اختياري)
                var logPath = Path.Combine(uploadsDir, "upload-log.txt");
                var debugInfo = $"[{DateTime.Now}] FileName={file.FileName}, Ext={ext}";
                System.IO.File.AppendAllText(logPath, debugInfo + Environment.NewLine);

                var imagePath = $"/images/uploads/{fileName}";
                var customerId = GetCustomerId();

                // حفظ الروشتة في الداتا
                if (chat.PharmacyId.HasValue)
                {
                    var prescription = new Prescription
                    {
                        CustomerId = customerId,
                        PharmId = chat.PharmacyId.Value,
                        Name = "Prescription from chat",
                        OrderId = null,
                        Image = imagePath,
                        Status = PrescriptionStatuses.Uploaded
                    };

                    _context.Prescriptions.Add(prescription);
                    _context.SaveChanges();
                }

                // رسالة انجليزي تلقائية لو مفيش تكست
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Prescription uploaded successfully 🧾💊";
                }
            }

            // --- حفظ رسالة الشات ---
            var msg = new ChatMessage
            {
                ChatId = chatId,
                SenderType = "Customer",
                MessageText = message ?? "",
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            return RedirectToAction("Index", new { pharmacyId = chat.PharmacyId });
        }
    }
}
