using System.Collections.Generic;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business
{
    public interface IOrderItemManager
    {
        IEnumerable<OrderItem> GetAllOrderItems();
        OrderItem? GetOrderItem(int orderId, int productId);
        IEnumerable<OrderItem> GetItemsByOrder(int orderId);
        IEnumerable<OrderItem> GetItemsByProduct(int productId);
        void CreateOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void DeleteOrderItem(int orderId, int productId);
        void UpdateItemQuantity(int orderId, int productId, int quantity);
        void UpdateItemStatus(int orderId, int productId, string newStatus);
    }
}
