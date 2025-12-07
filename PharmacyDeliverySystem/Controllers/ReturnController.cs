using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Returns;

namespace PharmacyDeliverySystem.Controllers
{
    public class ReturnController : Controller
    {
        private readonly IReturnManager _returnManager;

        public ReturnController(IReturnManager returnManager)
        {
            _returnManager = returnManager;
        }

        // ======== Admin list ========
        public IActionResult Index()
        {
            var items = _returnManager.GetAll();
            return View(items);
        }

        public IActionResult Details(int id)
        {
            var item = _returnManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ======== Create (Customer Request Return) ========

        // GET: /Return/Create?orderId=5
        [HttpGet]
        public IActionResult Create(int orderId)
        {
            var vm = new ReturnCreateVm
            {
                OrderId = orderId,      // جاي من MyOrderDetails
                Status = "Requested"   // القيمة الافتراضية لطلب الإرجاع
            };

            return View(vm);
        }

        // POST: /Return/Create
        [HttpPost]
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
                              ? "Requested"
                              : vm.Status
            };

            _returnManager.Add(entity);

            // رسالة للكاستمر إنه الطلب اتسجّل
            TempData["ReturnMessage"] = "Your return request has been submitted.";

            // ✅ رجّع الكاستمر لصفحة تفاصيل الأوردر بتاعه
            return RedirectToAction("MyOrderDetails", "Order", new { id = vm.OrderId });
        }

        // ======== Edit / Status / Delete (Admin) ========

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _returnManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Return model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _returnManager.Update(model);
            return RedirectToAction(nameof(Details), new { id = model.ReturnId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStatus(int id, string status)
        {
            _returnManager.SetStatus(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _returnManager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
