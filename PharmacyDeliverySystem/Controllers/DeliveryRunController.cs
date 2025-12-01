using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.DeliveryRun;
using System.Linq;
using System.Security.Claims;

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
            // كل الأوردرز اللي Status = Pending
            var pendingOrders = _orderManager.GetPendingOrders();

            var model = new DeliveryRunViewModels.CreateDeliveryRunViewModel
            {
                // مش مهم هنا نعبيها، اللي يهمنا في POST هو اللي جاي من الـ Checkboxes
                OrderIds = pendingOrders.Select(o => o.OrderId).ToList()
            };

            ViewBag.PendingOrders = pendingOrders;
            return View(model);
        }

        // =============================
        // 2) POST: Create Delivery Run
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DeliveryRunViewModels.CreateDeliveryRunViewModel model)
        {
            // لازم يكون فيه رايدر ID و أوردرز متعلّمة
            if (model.OrderIds == null || !model.OrderIds.Any())
            {
                ModelState.AddModelError("OrderIds", "اختر على الأقل Order واحد للـ Delivery Run.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PendingOrders = _orderManager.GetPendingOrders();
                return View(model);
            }

            // 1) نجيب PharmacyId من الـ User لو هو صيدلي
            int? pharmacyId = null;
            if (User.IsInRole("Pharmacy"))
            {
                // على أساس إنك حاطة الـ PharmId في ClaimTypes.NameIdentifier
                var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(idString, out var pid))
                {
                    pharmacyId = pid;
                }
            }

            var orderIds = model.OrderIds ?? new List<int>();

            var selectedOrders = _orderManager
                .GetOrdersByIds(orderIds)
                .ToList();

            if (!selectedOrders.Any())
            {
                ModelState.AddModelError("", "لم يتم العثور على أي Orders مطابقة للاختيارات.");
                ViewBag.PendingOrders = _orderManager.GetPendingOrders();
                return View(model);
            }

            // 3) نعمل DeliveryRun جديد
            var run = new DeliveryRun
            {
                RiderId = model.RiderId,
                StartAt = System.DateTime.Now,
                Orders = selectedOrders  // EF هيربطهم تلقائياً
            };

            _runManager.CreateRun(run); // هنا RunId يتولّد من الـ DB

            // 4) نحدّث كل Order: RunId + Status + PharmId
            foreach (var order in selectedOrders)
            {
                order.Status = "OnDelivery";
                order.RunId = run.RunId;

                if (pharmacyId.HasValue)
                {
                    order.PharmId = pharmacyId.Value;  // 👈 هنا بنسجّل الصيدلي
                }

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

        // تفاصيل Run معيّن
        public IActionResult Details(int id)
        {
            var run = _runManager
                .GetActiveRuns()
                .FirstOrDefault(r => r.RunId == id);

            if (run == null)
                return NotFound();

            return View(run); // Views/DeliveryRun/Details.cshtml
        }

        // =============================
        // 4) POST: Complete Run
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int runId)
        {
            _runManager.CompleteRun(runId);
            return RedirectToAction("Index");
        }
    }
}
