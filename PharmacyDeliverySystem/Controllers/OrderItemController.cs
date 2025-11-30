using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Controllers
{
    public class OrderItemController : Controller
    {
        private readonly IOrderItemManager _manager;

        public OrderItemController(IOrderItemManager manager)
        {
            _manager = manager;
        }

        // GET: OrderItem
        public IActionResult Index()
        {
            var items = _manager.GetAllOrderItems();
            return View(items); // Views/OrderItem/Index.cshtml
        }

        // GET: OrderItem/Details/{orderId}/{productId}
        public IActionResult Details(int orderId, int productId)
        {
            var item = _manager.GetOrderItem(orderId, productId);
            if (item == null)
                return NotFound();

            return View(item); // Views/OrderItem/Details.cshtml
        }

        // GET: OrderItem/Create
        public IActionResult Create()
        {
            return View(); // Views/OrderItem/Create.cshtml
        }

        // POST: OrderItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderItem item)
        {
            if (ModelState.IsValid)
            {
                _manager.CreateOrderItem(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: OrderItem/Edit/{orderId}/{productId}
        public IActionResult Edit(int orderId, int productId)
        {
            var item = _manager.GetOrderItem(orderId, productId);
            if (item == null)
                return NotFound();

            return View(item); // Views/OrderItem/Edit.cshtml
        }

        // POST: OrderItem/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderItem item)
        {
            if (ModelState.IsValid)
            {
                _manager.UpdateOrderItem(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: OrderItem/Delete/{orderId}/{productId}
        public IActionResult Delete(int orderId, int productId)
        {
            var item = _manager.GetOrderItem(orderId, productId);
            if (item == null)
                return NotFound();

            return View(item); // Views/OrderItem/Delete.cshtml
        }

        // POST: OrderItem/Delete/{orderId}/{productId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int orderId, int productId)
        {
            _manager.DeleteOrderItem(orderId, productId);
            return RedirectToAction(nameof(Index));
        }

        // GET: OrderItem/ByOrder/{orderId}
        public IActionResult ByOrder(int orderId)
        {
            var items = _manager.GetItemsByOrder(orderId);
            return View("Index", items); // reuse Index view
        }

        // GET: OrderItem/ByProduct/{productId}
        public IActionResult ByProduct(int productId)
        {
            var items = _manager.GetItemsByProduct(productId);
            return View("Index", items); // reuse Index view
        }

        // POST: OrderItem/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int orderId, int productId, int quantity)
        {
            _manager.UpdateItemQuantity(orderId, productId, quantity);
            return RedirectToAction(nameof(Details), new { orderId, productId });
        }

        // POST: OrderItem/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int orderId, int productId, string status)
        {
            _manager.UpdateItemStatus(orderId, productId, status);
            return RedirectToAction(nameof(Details), new { orderId, productId });
        }
    }
}
