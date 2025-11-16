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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReturnCreateVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReturnCreateVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = new Returnn
            {
                OrderID = vm.OrderId,
                Reason = vm.Reason,
                Status = vm.Status
            };

            _returnManager.Add(entity);
            return RedirectToAction(nameof(Details), new { id = entity.ReturnId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _returnManager.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Returnn model)
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
