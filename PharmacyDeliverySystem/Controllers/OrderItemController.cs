using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemManager _manager;

        public OrderItemController(IOrderItemManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderItem>> GetAll()
        {
            var items = _manager.GetAllOrderItems();
            return Ok(items);
        }

        [HttpGet("{orderId}/{productId}")]
        public ActionResult<OrderItem> Get(int orderId, int productId)
        {
            var item = _manager.GetOrderItem(orderId, productId);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("ByOrder/{orderId}")]
        public ActionResult<IEnumerable<OrderItem>> GetByOrder(int orderId)
        {
            var items = _manager.GetItemsByOrder(orderId);
            return Ok(items);
        }

        [HttpGet("ByProduct/{productId}")]
        public ActionResult<IEnumerable<OrderItem>> GetByProduct(int productId)
        {
            var items = _manager.GetItemsByProduct(productId);
            return Ok(items);
        }

        [HttpPost]
        public ActionResult Create([FromBody] OrderItem item)
        {
            _manager.CreateOrderItem(item);
            return CreatedAtAction(nameof(Get), new { orderId = item.OrderId, productId = item.ProductId }, item);
        }

        [HttpPut]
        public ActionResult Update([FromBody] OrderItem item)
        {
            _manager.UpdateOrderItem(item);
            return NoContent();
        }

        [HttpDelete("{orderId}/{productId}")]
        public ActionResult Delete(int orderId, int productId)
        {
            _manager.DeleteOrderItem(orderId, productId);
            return NoContent();
        }

        [HttpPatch("UpdateQuantity/{orderId}/{productId}/{quantity}")]
        public ActionResult UpdateQuantity(int orderId, int productId, int quantity)
        {
            _manager.UpdateItemQuantity(orderId, productId, quantity);
            return NoContent();
        }

        [HttpPatch("UpdateStatus/{orderId}/{productId}/{status}")]
        public ActionResult UpdateStatus(int orderId, int productId, string status)
        {
            _manager.UpdateItemStatus(orderId, productId, status);
            return NoContent();
        }
    }
}
