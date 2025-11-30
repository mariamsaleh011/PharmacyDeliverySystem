using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IOrderManager
    {
        IEnumerable<Order> GetAllOrders();

        Order? GetOrderById(int id);

        IEnumerable<Order> GetOrdersByCustomer(int customerId);

        IEnumerable<Order> GetOrdersByPharmacy(int pharmacyId);

        IEnumerable<Order> GetOrdersByStatus(string status);

        // ✅ عشان نجيب الأوردرات الـ Pending
        IEnumerable<Order> GetPendingOrders();

        decimal GetOrderTotal(int orderId);

        void UpdateOrderStatus(int orderId, string newStatus);

        void AssignOrderToDeliveryRun(int orderId, int runId);

        void CreateOrder(Order order);

        void UpdateOrder(Order order);

        void CancelOrder(int id);

        // ✅ مهمة لـ DeliveryRun
        IEnumerable<Order> GetOrdersByIds(List<int> orderIds);
    }
}
