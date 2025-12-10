using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.ViewModels.Order;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

            if (order.CreatedAt == default)
                order.CreatedAt = DateTime.Now;

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

        // ===== Update order status + items status =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            if (order.Status != newStatus)
            {
                order.Status = newStatus;

                foreach (var item in order.OrderItems)
                {
                    item.Status = newStatus;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignOrderToDeliveryRun(int orderId, int runId)
        {
            _orderManager.AssignOrderToDeliveryRun(orderId, runId);
            return RedirectToAction("Details", new { id = orderId });
        }

        // ===== Pharmacy Orders ======
        [Authorize(Roles = "Pharmacy")]
        public IActionResult PharmacyOrders()
        {
            var orders = _context.Orders
                                 .Include(o => o.Customer)
                                 .OrderByDescending(o => o.CreatedAt)
                                 .AsNoTracking()
                                 .ToList();

            return View("PharmacyOrders", orders);
        }

        // ===== Mark order as Delivered (Pharmacy) + items =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pharmacy")]
        public IActionResult MarkAsDelivered(int id)
        {
            var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            if (order.Status != "Delivered")
            {
                order.Status = "Delivered";

                foreach (var item in order.OrderItems)
                {
                    item.Status = "Delivered";
                }

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(PharmacyOrders));
        }

        // ========== Checkout ==========

        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutViewModel model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
            {
                return BadRequest(new { success = false, message = "Cart is empty." });
            }

            int customerId;

            // لو اللي عامل Checkout هو الفارمسي
            if (User.IsInRole("Pharmacy"))
            {
                if (!model.CustomerId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Customer id is required for pharmacy orders." });
                }

                var customerFromPharmacy = _context.Customers
                    .FirstOrDefault(c => c.CustomerId == model.CustomerId.Value);

                if (customerFromPharmacy == null)
                {
                    return BadRequest(new { success = false, message = "Customer not found in database." });
                }

                customerId = customerFromPharmacy.CustomerId;
            }
            // غير كده → كاستمر عادي (من الـ Claims)
            else
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { success = false, message = "Please login as customer first." });
                }

                var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
                if (customer == null)
                {
                    return BadRequest(new { success = false, message = "Customer not found in database." });
                }

                customerId = customer.CustomerId;
            }

            var totalQuantity = model.Items.Sum(i => i.Quantity);
            var total = model.Items.Sum(i => i.Price * i.Quantity);

            int? pharmId = null;
            var firstItem = model.Items.FirstOrDefault();

            if (firstItem != null)
            {
                pharmId = _context.Products
                    .Where(p => p.ProId == firstItem.ProductId)
                    .Select(p => (int?)p.PharmId)
                    .FirstOrDefault();
            }

            var invoiceNo = (int)(DateTime.UtcNow.Ticks % int.MaxValue);

            var order = new Order
            {
                CustomerId = customerId,
                PharmId = pharmId,
                Status = "Pending",
                TotalPrice = total,
                Price = total,
                Quantity = totalQuantity.ToString(),
                InvoiceNo = invoiceNo,
                PdfUrl = "",
                CreatedAt = DateTime.Now
            };

            _orderManager.CreateOrder(order);
            var newOrderId = order.OrderId;

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

            var redirectUrl = Url.Action(
                "InvoiceDetails",
                "QrConfirmation",
                new { orderId = newOrderId, isPharmacy = User.IsInRole("Pharmacy") }
            );

            return Json(new { success = true, redirectUrl });
        }

        // ========== My Orders ==========

        public IActionResult MyOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
                return Unauthorized();

            var orders = _context.Orders
                .Where(o => o.CustomerId == customer.CustomerId)
                .OrderByDescending(o => o.OrderId)
                .ToList();

            return View("MyOrders", orders);
        }

        public IActionResult MyOrderDetails(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
                return Unauthorized();

            var order = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Returns)
                .FirstOrDefault(o => o.OrderId == id && o.CustomerId == customer.CustomerId);

            if (order == null)
                return NotFound();

            return View("MyOrderDetails", order);
        }
    }
}
