using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class OrderManager : IOrderManager
    {
        private readonly PharmacyDeliveryContext _context;

        public OrderManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        private IQueryable<Order> IncludeAll()
            => _context.Orders
                       .Include(o => o.Customer)
                       .Include(o => o.Payment)
                       .Include(o => o.Pharm)
                       .Include(o => o.Run);
        // .Include(o => o.OrderItems);  // 👈 متشالة مؤقتًا

        public IEnumerable<Order> GetAllOrders()
            => IncludeAll().AsNoTracking().ToList();

        public Order? GetOrderById(int id)
            => IncludeAll().FirstOrDefault(o => o.OrderId == id);

        public IEnumerable<Order> GetOrdersByCustomer(int customerId)
            => IncludeAll()
               .Where(o => o.CustomerId == customerId)
               .AsNoTracking()
               .ToList();

        public IEnumerable<Order> GetOrdersByPharmacy(int pharmacyId)
            => IncludeAll()
               .Where(o => o.PharmId == pharmacyId)
               .AsNoTracking()
               .ToList();

        public IEnumerable<Order> GetOrdersByStatus(string status)
            => IncludeAll()
               .Where(o => o.Status == status)
               .AsNoTracking()
               .ToList();

        public IEnumerable<Order> GetPendingOrders()
        {
            return GetOrdersByStatus("Pending");
        }

        public decimal GetOrderTotal(int orderId)
        {
            var order = IncludeAll().FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return 0m;

            return order.TotalPrice ?? 0;
        }

        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return;

            order.Status = newStatus;
            _context.SaveChanges();
        }

        public void AssignOrderToDeliveryRun(int orderId, int runId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return;

            order.RunId = runId;
            _context.SaveChanges();
        }

        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void CancelOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return;

            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        public IEnumerable<Order> GetOrdersByIds(List<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
                return new List<Order>();

            return IncludeAll()
                   .Where(o => orderIds.Contains(o.OrderId))
                   .ToList();
        }
    }
}
