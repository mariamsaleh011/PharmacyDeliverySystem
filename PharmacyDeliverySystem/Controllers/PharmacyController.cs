using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Controllers
{
    public class PharmacyController : Controller
    {
        private readonly IPharmacyManager _manager;

        public PharmacyController(IPharmacyManager manager)
        {
            _manager = manager;
        }

        // GET: Pharmacy
        public IActionResult Index()
        {
            var pharmacies = _manager.GetAllPharmacies();
            return View(pharmacies); // Views/Pharmacy/Index.cshtml
        }

        // GET: Pharmacy/Details/{id}
        public IActionResult Details(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy); // Views/Pharmacy/Details.cshtml
        }

        // GET: Pharmacy/Create
        public IActionResult Create()
        {
            return View(); // Views/Pharmacy/Create.cshtml
        }

        // POST: Pharmacy/Create
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

        // GET: Pharmacy/Edit/{id}
        public IActionResult Edit(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy); // Views/Pharmacy/Edit.cshtml
        }

        // POST: Pharmacy/Edit
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

        // GET: Pharmacy/Delete/{id}
        public IActionResult Delete(int id)
        {
            var pharmacy = _manager.GetById(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy); // Views/Pharmacy/Delete.cshtml
        }

        // POST: Pharmacy/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Pharmacy/ByName/{name}
        public IActionResult ByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return BadRequest();
            var pharmacies = _manager.GetByName(name);
            return View("Index", pharmacies); // reuse Index view
        }
    }
}
