using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Business;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class OrderItemManager : IOrderItemManager
    {
        private readonly PharmacyDeliveryContext _context;

        public OrderItemManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderItem> GetAllOrderItems()
        {
            return _context.OrderItems.ToList();
        }

        public OrderItem? GetOrderItem(int orderId, int productId)
        {
            return _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
        }

        public IEnumerable<OrderItem> GetItemsByOrder(int orderId)
        {
            return _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
        }

        public IEnumerable<OrderItem> GetItemsByProduct(int productId)
        {
            return _context.OrderItems.Where(oi => oi.ProductId == productId).ToList();
        }

        public void CreateOrderItem(OrderItem item)
        {
            _context.OrderItems.Add(item);
            _context.SaveChanges();
        }

        public void UpdateOrderItem(OrderItem item)
        {
            _context.OrderItems.Update(item);
            _context.SaveChanges();
        }

        public void DeleteOrderItem(int orderId, int productId)
        {
            var item = _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
            if (item == null) return;
            _context.OrderItems.Remove(item);
            _context.SaveChanges();
        }

        public void UpdateItemQuantity(int orderId, int productId, int quantity)
        {
            var item = _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
            if (item == null) return;
            item.Quantity = quantity;
            _context.SaveChanges();
        }

        public void UpdateItemStatus(int orderId, int productId, string newStatus)
        {
            var item = _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
            if (item == null) return;
            item.Status = newStatus;
            _context.SaveChanges();
        }
    }
}

