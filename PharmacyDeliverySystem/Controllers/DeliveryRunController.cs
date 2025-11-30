using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.DeliveryRun;
using System.Linq;

namespace PharmacyDeliverySystem.Controllers
{
    public class DeliveryRunController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IDeliveryRunManager _runManager;

        public DeliveryRunController(IOrderManager orderManager, IDeliveryRunManager runManager)
        {
            _orderManager = orderManager;
            _runManager = runManager;
        }

        // =============================
        // 1) GET: Create Delivery Run
        // =============================
        public IActionResult Create()
        {
            var pendingOrders = _orderManager.GetPendingOrders(); // Orders with Status = "Pending"
            var model = new DeliveryRunViewModels.CreateDeliveryRunViewModel
            {
                OrderIds = pendingOrders.Select(o => o.OrderId).ToList()
            };
            ViewBag.PendingOrders = pendingOrders; // لعرض أسماء العملاء في View
            return View(model);
        }

        // =============================
        // 2) POST: Create Delivery Run
        // =============================
        [HttpPost]
        public IActionResult Create(DeliveryRunViewModels.CreateDeliveryRunViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PendingOrders = _orderManager.GetPendingOrders();
                return View(model);
            }

            var selectedOrders = _orderManager.GetOrdersByIds(model.OrderIds);

            var run = new DeliveryRun
            {
                RiderId = model.RiderId,
                StartAt = System.DateTime.Now,
                Orders = selectedOrders
            };

            _runManager.CreateRun(run);

            foreach (var order in selectedOrders)
            {
                order.Status = "OnDelivery";
                order.RunId = run.RunId;
                _orderManager.UpdateOrder(order);
            }

            return RedirectToAction("Index");
        }

        // =============================
        // 3) GET: Active Runs
        // =============================
        public IActionResult Index()
        {
            var activeRuns = _runManager.GetActiveRuns();
            return View(activeRuns);
        }

        // =============================
        // 4) POST: Complete Run
        // =============================
        [HttpPost]
        public IActionResult Complete(int runId)
        {
            _runManager.CompleteRun(runId);
            return RedirectToAction("Index");
        }
    }
}

