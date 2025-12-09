using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Returns;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    [Authorize]
    public class ReturnController : Controller
    {
        private readonly IReturnManager _returnManager;

        public ReturnController(IReturnManager returnManager)
        {
            _returnManager = returnManager;
        }

        // ======== Admin / Pharmacy list ========
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Index()
        {
            var items = _returnManager.GetAll();
            return View(items);
        }

        [Authorize(Roles = "Pharmacy")]
        public IActionResult Details(int id)
        {
            var item = _returnManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ======== Create (Customer Request Return) ========

        // GET: /Return/Create?orderId=5
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public IActionResult Create(int orderId)
        {
            var vm = new ReturnCreateVm
            {
                OrderId = orderId,    // جاي من MyOrderDetails
                Status = "Pending"    // القيمة الافتراضية لطلب الإرجاع
            };

            return View(vm);
        }

        // POST: /Return/Create
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReturnCreateVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = new Return
            {
                OrderId = vm.OrderId,
                Reason = vm.Reason,
                Status = string.IsNullOrWhiteSpace(vm.Status)
                              ? "Pending"
                              : vm.Status
            };

            _returnManager.Add(entity);

            // رسالة للكاستمر إنه الطلب اتسجّل
            TempData["ReturnMessage"] = "Your return request has been submitted.";

            // رجّع الكاستمر لصفحة تفاصيل الأوردر بتاعه
            return RedirectToAction("MyOrderDetails", "Order", new { id = vm.OrderId });
        }

        // ======== Edit / Status / Delete (Pharmacy) ========

        [HttpGet]
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Edit(int id)
        {
            var item = _returnManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Return model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _returnManager.Update(model);
            return RedirectToAction(nameof(Details), new { id = model.ReturnId });
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                status = "Pending";
            }

            _returnManager.SetStatus(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _returnManager.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // ======== Pending list for Pharmacy Dashboard ========

        // /Return/Pending
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Pending()
        {
            // لو عندك حالات قديمة باسم Requested هنحسبها برضه كـ Pending
            var items = _returnManager
                .GetAll()
                .Where(r => r.Status == "Pending" || r.Status == "Requested")
                .ToList();

            return View(items);
        }

        // ======== Quick Approve / Reject buttons ========

        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            _returnManager.SetStatus(id, "Approved");
            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            _returnManager.SetStatus(id, "Rejected");
            return RedirectToAction(nameof(Pending));
        }
    }
}
