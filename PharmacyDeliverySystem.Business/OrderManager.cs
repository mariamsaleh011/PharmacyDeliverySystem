using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business;

public class OrderManager : IOrderManager
{
    private readonly PharmacyDeliveryContext  context;

    public OrderManager(PharmacyDeliveryContext context)
    {
        this.context = context;
    }
    //Include is used for loading related objects (via foreign keys or navigation collections) in one query.
    //FirstOrDefault
         // Purpose: Finds the first item in a collection/query that matches a condition - Return Type: A single object (or null).
    //Where
         //Purpose: Finds all items matching a condition; can be zero, one, or many - Return Type: An IEnumerable<T> (a collection/sequence).
    
    public IEnumerable<Order> GetAllOrders()
    {
        return context.Orders
            .Include(o => o.Pharm)
            .Include(o => o.Run)
            .Include(o => o.Customers)
            .ToList();
    }

    public Order? GetOrderById(int id)
    {
        return context.Orders
            .Include (o => o.Pharm)
            .Include (o => o.Run)
            .Include (o => o.Customers)
            .Include(o => o.Order_Invoice)
            .Include(o => o.OrderProducts)
                   .ThenInclude(op => op.Pro)
            .FirstOrDefault(o =>o.OrderID == id);
    }

    public IEnumerable<Order> GetOrdersByCustomer(int customerId)
    {
        return context.Orders
            .Where(o => o.Customers.Any(c => c.CustomerID == customerId))
            .ToList();
    }

    public IEnumerable<Order> GetOrdersByPharmacy(int pharmacyId)
    {
        return context.Orders
            .Where(o => o.PharmId == pharmacyId)
            .Include(o => o.Pharm)
            .Include(o => o.Customers)
            .ToList();
    }

    public IEnumerable<Order> GetOrdersByStatus(string status)
    {
        return context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.Customers)
            .ToList();
    }

    public decimal GetOrderTotal(int orderId)
    {
        var order= context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.OrderID == orderId);

        return order?.Price ?? 0;

    }

    public void UpdateOrderStatus(int orderId, string newStatus)
    {
        var order = context.Orders.Find(orderId);
        if (order != null)
        {
            order.Status = newStatus;
            context.SaveChanges();
        }
    }

    public void AssignOrderToDeliveryRun(int orderId, int runId)
    {
        var order = context.Orders.Find(orderId);
        if (order != null)
        {
            order.RunId = runId;
            context.SaveChanges();
        }
    }

    public void CreateOrder(Order order)
    {
        context.Orders.Add(order);
        context.SaveChanges();
    }

    public void UpdateOrder(Order order)
    {
        context.Orders.Update(order);
        context.SaveChanges();
    }

    public void CancelOrder(int id)
    {
        var order = context.Orders.Find(id);
        if (order != null)
        {
            order.Status = "Canceled";
            context.SaveChanges();
        }
    }
}