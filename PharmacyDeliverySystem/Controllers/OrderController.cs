using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Order;
using System.Linq;   // عشان Sum و Any

namespace PharmacyDeliverySystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderManager _orderManager;

        public OrderController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        // GET : list of orders
        public IActionResult Index()
        {
            var orders = _orderManager.GetAllOrders();
            return View(orders);
        }

        // GET : order details by Id
        public IActionResult Details(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET : orders by customer
        public IActionResult ByCustomer(int customerId)
        {
            var orders = _orderManager.GetOrdersByCustomer(customerId);
            return View("Index", orders);
        }

        // GET : orders by pharmacy
        public IActionResult ByPharmacy(int pharmacyId)
        {
            var orders = _orderManager.GetOrdersByPharmacy(pharmacyId);
            return View("Index", orders);
        }

        // GET : orders by status
        public IActionResult ByStatus(string status)
        {
            var orders = _orderManager.GetOrdersByStatus(status);
            return View("Index", orders);
        }

        // GET : show create form
        public IActionResult Create()
        {
            return View();
        }

        // POST : create new order (لو من الـ Admin أو Dashboard)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderManager.CreateOrder(order);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET : show edit form
        public IActionResult Edit(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST : update order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderManager.UpdateOrder(order);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET : show delete confirmation page
        public IActionResult Delete(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST : actually delete after confirmation
        [HttpPost, ActionName("CancelOrder")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderManager.CancelOrder(id);
            return RedirectToAction("Index");
        }

        // POST : update order status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            _orderManager.UpdateOrderStatus(orderId, newStatus);
            return RedirectToAction("Details", new { id = orderId });
        }

        // POST : assign order to delivery run
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignOrderToDeliveryRun(int orderId, int runId)
        {
            _orderManager.AssignOrderToDeliveryRun(orderId, runId);
            return RedirectToAction("Details", new { id = orderId });
        }

        // ==========================
        // Checkout: يستقبل السلة من الفرونت
        // ==========================
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutViewModel model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
                return BadRequest("Cart is empty");

            var total = model.Items.Sum(i => i.Price * i.Quantity);

            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = "Pending",
                TotalPrice = total
            };

            _orderManager.CreateOrder(order);   // هنا بيتحفظ في الـ DB
            var newOrderId = order.OrderId;

            var redirectUrl = Url.Action(
                action: "InvoiceDetails",
                controller: "QrConfirmation",
                values: new { orderId = newOrderId });

            return Json(new { success = true, redirectUrl });
        }

    }
}
