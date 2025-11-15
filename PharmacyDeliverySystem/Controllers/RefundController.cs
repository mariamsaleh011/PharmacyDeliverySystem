using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Refunds;

namespace PharmacyDeliverySystem.Controllers
{
    public class RefundController : Controller
    {
        private readonly IRefundManager _refundManager;

        public RefundController(IRefundManager refundManager)
        {
            _refundManager = refundManager;
        }

        public IActionResult Index()
        {
            var items = _refundManager.GetAll();
            return View(items);
        }

        public IActionResult Details(int id)
        {
            var item = _refundManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new RefundCreateVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RefundCreateVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = new Refund
            {
                PayId = vm.PayId,
                Reason = vm.Reason
            };

            _refundManager.Add(entity);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _refundManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Refund model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _refundManager.Update(model);
            return RedirectToAction(nameof(Details), new { id = model.RefId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _refundManager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
