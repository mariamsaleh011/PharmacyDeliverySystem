using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Order;
using System.Linq;
using System.Security.Claims;

namespace PharmacyDeliverySystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IOrderItemManager _orderItemManager;
        private readonly PharmacyDeliveryContext _context;

        public OrderController(
            IOrderManager orderManager,
            IOrderItemManager orderItemManager,
            PharmacyDeliveryContext context)
        {
            _orderManager = orderManager;
            _orderItemManager = orderItemManager;
            _context = context;
        }

        // ========== CRUD & Filters ==========

        public IActionResult Index()
        {
            var orders = _orderManager.GetAllOrders();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult ByCustomer(int customerId)
        {
            var orders = _orderManager.GetOrdersByCustomer(customerId);
            return View("Index", orders);
        }

        public IActionResult ByPharmacy(int pharmacyId)
        {
            var orders = _orderManager.GetOrdersByPharmacy(pharmacyId);
            return View("Index", orders);
        }

        public IActionResult ByStatus(string status)
        {
            var orders = _orderManager.GetOrdersByStatus(status);
            return View("Index", orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);

            _orderManager.CreateOrder(order);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);

            _orderManager.UpdateOrder(order);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var order = _orderManager.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("CancelOrder")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderManager.CancelOrder(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            _orderManager.UpdateOrderStatus(orderId, newStatus);
            return RedirectToAction("Details", new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignOrderToDeliveryRun(int orderId, int runId)
        {
            _orderManager.AssignOrderToDeliveryRun(orderId, runId);
            return RedirectToAction("Details", new { id = orderId });
        }

        // ========== Checkout من الـ Cart ==========
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutViewModel model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
            {
                return BadRequest(new { success = false, message = "Cart is empty." });
            }

            // 1) نجيب الإيميل من الـ Cookie Authentication
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { success = false, message = "Please login as customer first." });
            }

            // 2) نلاقي الـ Customer في الداتابيز
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
            {
                return BadRequest(new { success = false, message = "Customer not found in database." });
            }

            // 3) نحسب إجمالي الكمية وإجمالي السعر
            var totalQuantity = model.Items.Sum(i => i.Quantity);
            var total = model.Items.Sum(i => i.Price * i.Quantity);

            // 4) نجيب الـ PharmId من أول منتج في السلة
            int? pharmId = null;
            var firstItem = model.Items.FirstOrDefault();
            if (firstItem != null)
            {
                pharmId = _context.Products
                    .Where(p => p.ProId == firstItem.ProductId)
                    .Select(p => (int?)p.PharmId)   // نفترض إن عندك عمود PharmId في جدول Products
                    .FirstOrDefault();
            }


            var invoiceNo = (int)(DateTime.UtcNow.Ticks % int.MaxValue);

            // 5) ننشئ Order جديد مربوط بالـ Customer الحقيقي و الـ Pharmacy
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                PharmId = pharmId,          // 👈 هنا بقى مش هتبقى NULL

                Status = "Pending",
                TotalPrice = total,
                Price = total,
                Quantity = totalQuantity.ToString(),   // أو حوّلي العمود في الموديل لـ int

                InvoiceNo = invoiceNo,
                PdfUrl = string.Empty
            };

            _orderManager.CreateOrder(order);
            var newOrderId = order.OrderId;

            // 6) نحفظ الـ OrderItems
            foreach (var item in model.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = newOrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Status = "Pending"
                };

                _orderItemManager.CreateOrderItem(orderItem);
            }

            // 7) نرجّع لينك الفاتورة
            var redirectUrl = Url.Action(
                action: "InvoiceDetails",
                controller: "QrConfirmation",
                values: new { orderId = newOrderId });

            return Json(new { success = true, redirectUrl });
        }

    }
}
