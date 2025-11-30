using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentManager _manager;

        public PaymentController(IPaymentManager manager)
        {
            _manager = manager;
        }

        // GET: Payment
        public IActionResult Index()
        {
            var payments = _manager.GetAllPayments();
            return View(payments); // returns Views/Payment/Index.cshtml
        }

        // GET: Payment/Details/{payId}
        public IActionResult Details(string payId)
        {
            if (string.IsNullOrEmpty(payId))
                return BadRequest();

            var payment = _manager.GetPaymentById(payId);
            if (payment == null)
                return NotFound();

            return View(payment); // returns Views/Payment/Details.cshtml
        }

        // GET: Payment/Create
        public IActionResult Create()
        {
            return View(); // returns Views/Payment/Create.cshtml
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                _manager.CreatePayment(payment);
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payment/Edit/{payId}
        public IActionResult Edit(string payId)
        {
            if (string.IsNullOrEmpty(payId))
                return BadRequest();

            var payment = _manager.GetPaymentById(payId);
            if (payment == null)
                return NotFound();

            return View(payment); // returns Views/Payment/Edit.cshtml
        }

        // POST: Payment/Edit/{payId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                _manager.UpdatePayment(payment);
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payment/Delete/{payId}
        public IActionResult Delete(string payId)
        {
            if (string.IsNullOrEmpty(payId))
                return BadRequest();

            var payment = _manager.GetPaymentById(payId);
            if (payment == null)
                return NotFound();

            return View(payment); // returns Views/Payment/Delete.cshtml
        }

        // POST: Payment/Delete/{payId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string payId)
        {
            _manager.DeletePayment(payId);
            return RedirectToAction(nameof(Index));
        }

        // Optional: Filter by status
        // GET: Payment/ByStatus/{status}
        public IActionResult ByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest();

            var payments = _manager.GetPaymentsByStatus(status);
            return View("Index", payments); // reuse Index.cshtml to show filtered results
        }
    }
}
