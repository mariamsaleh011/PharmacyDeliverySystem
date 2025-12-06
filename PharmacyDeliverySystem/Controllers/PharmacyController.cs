using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Security.Claims;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyController : Controller
    {
        private readonly IPharmacyManager _manager;

        public PharmacyController(IPharmacyManager manager)
        {
            _manager = manager;
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

            return View(chat); // Views/Pharmacy/OpenChat.cshtml
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

        // =========================
        // Admin/Owner Actions
        // =========================

        // GET: Add Admin form
        public IActionResult AddAdmin()
        {
            return View(); // Views/Pharmacy/AddAdmin.cshtml
        }

        // POST: Add Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAdmin(PharmacyAdmin admin)
        {
            int pharmId = GetPharmacyId();
            if (pharmId == 0) return Unauthorized();

            admin.PharmId = pharmId;
            admin.CreatedAt = DateTime.Now;

            _manager.CreateAdmin(admin); // لازم تضيفي Method في IPharmacyManager وBusiness Layer
            return RedirectToAction("Dashboard");
        }

        // GET: List all Admins for this pharmacy
        public IActionResult ListAdmins()
        {
            int pharmId = GetPharmacyId();
            if (pharmId == 0) return Unauthorized();

            var admins = _manager.GetAdminsByPharmacyId(pharmId); // Business Layer
            return View(admins); // Views/Pharmacy/ListAdmins.cshtml
        }

        public IActionResult Dashboard()
        {
            int pharmId = GetPharmacyId(); // دالة بتجيب PharmId للصيدلية المسجلة دخول
            if (pharmId == 0)
                return RedirectToAction("Login", "PharmacyAuth");

            // مثال: بيانات بسيطة للـ Dashboard
            ViewBag.TotalOrders = _manager.GetOrdersCount(pharmId);
            ViewBag.TotalProducts = _manager.GetProductsCount(pharmId);

            return View(); // MVC هيدور على Views/Pharmacy/Dashboard.cshtml
        }


    }
}
