//using System.Collections.Generic;
//using PharmacyDeliverySystem.Models;

//namespace PharmacyDeliverySystem.Business.Interfaces
//{
//    public interface IOrderManager 
//    {
//        IEnumerable<Order> GetAll();
//        Order? GetById(int id);
//        IEnumerable<Order> GetByCustomer(int customerId);
//        void Add(Order order);
//        void Update(Order order);
//        void Delete(int id);
//    }
//}
using PharmacyDeliverySystem.Models;


namespace PharmacyDeliverySystem.Business;

public interface IOrderManager
{
    IEnumerable<Order> GetAllOrders();
    Order? GetOrderById(int id);
    IEnumerable<Order> GetOrdersByCustomer(int customerId);
    IEnumerable<Order> GetOrdersByPharmacy(int pharmacyId);
    IEnumerable<Order> GetOrdersByStatus(string status);
    decimal GetOrderTotal(int orderId);
    void UpdateOrderStatus(int orderId, string newStatus);
    void AssignOrderToDeliveryRun(int orderId, int runId);
    void CreateOrder(Order order);
    void UpdateOrder(Order order);
    void CancelOrder(int id);
}

