using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Security.Claims;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyController : Controller
    {
        private readonly IPharmacyManager _manager;
        private readonly PharmacyDeliveryContext _context;

        public PharmacyController(IPharmacyManager manager, PharmacyDeliveryContext context)
        {
            _manager = manager;
            _context = context;
        }

        // =========================
        // Helper: Get Logged-in Pharmacy Id
        // =========================
        private int GetPharmacyId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return 0;

            var pharmacy = _manager.GetPharmacyByEmail(email);
            return pharmacy?.PharmId ?? 0;
        }

        // =========================
        // CRUD Actions for Pharmacy
        // =========================

        public IActionResult Index()
        {
            var pharmacies = _manager.GetAllPharmacies();
            return View(pharmacies);
        }

        public IActionResult Details(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                _manager.Create(pharmacy);
                return RedirectToAction(nameof(Index));
            }
            return View(pharmacy);
        }

        public IActionResult Edit(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                _manager.Update(pharmacy);
                return RedirectToAction(nameof(Index));
            }
            return View(pharmacy);
        }

        public IActionResult Delete(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return BadRequest();
            var pharmacies = _manager.GetByName(name);
            return View("Index", pharmacies);
        }

        // =========================
        // Pharmacy Chat Actions
        // =========================

        // List all chats for logged-in pharmacy
        public IActionResult Chats()
        {
            int pharmacyId = GetPharmacyId();
            if (pharmacyId == 0)
                return RedirectToAction("Login", "PharmacyAuth");

            var chats = _manager.GetChatsByPharmacyId(pharmacyId);
            return View(chats); // Views/Pharmacy/Chats.cshtml
        }

        // Open a specific chat
        public IActionResult OpenChat(int chatId)
        {
            int pharmacyId = GetPharmacyId();
            if (pharmacyId == 0)
                return RedirectToAction("Login", "PharmacyAuth");

            var chat = _manager.GetChatById(chatId);
            if (chat == null || chat.PharmacyId != pharmacyId)
                return NotFound();

            // ================================
            // ❗ إصلاح: جلب الروشتة الصحيحة
            // ================================
            Prescription? lastPrescription = null;

            if (chat.CustomerId != null)
            {
                lastPrescription = _context.Prescriptions
                    .Where(p => p.CustomerId == chat.CustomerId
                             && p.PharmId == pharmacyId)
                    .OrderByDescending(p => p.PreId)
                    .FirstOrDefault();
            }

            ViewBag.LastPrescription = lastPrescription;

            return View(chat);
        }

        [Authorize(Roles = "Pharmacy")]
        public IActionResult DebugPrescriptions()
        {
            var list = _context.Prescriptions
                .OrderByDescending(p => p.PreId)
                .Select(p => new
                {
                    p.PreId,
                    p.CustomerId,
                    p.PharmId,
                    p.Image
                })
                .ToList();

            return Json(list);
        }

        // Send message from pharmacy
        [HttpPost]
        public IActionResult SendMessage(int chatId, string message)
        {
            int pharmacyId = GetPharmacyId();
            if (pharmacyId == 0)
                return RedirectToAction("Login", "PharmacyAuth");

            if (!string.IsNullOrEmpty(message))
                _manager.SendMessage(chatId, message, "Pharmacy");

            return RedirectToAction("OpenChat", new { chatId = chatId });
        }
    }
}
