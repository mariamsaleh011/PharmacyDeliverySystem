using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class DeliveryRunManager : IDeliveryRunManager
    {
        private readonly PharmacyDeliveryContext _context;

        public DeliveryRunManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // ==============================
        // Create a new Delivery Run
        // ==============================
        public void CreateRun(DeliveryRun run)
        {
            if (run == null)
                throw new ArgumentNullException(nameof(run), "DeliveryRun object cannot be null");

            if (run.RiderId <= 0)
                throw new ArgumentException("RiderId must be valid");

            if (run.Orders == null || !run.Orders.Any())
                throw new ArgumentException("DeliveryRun must have at least one order");

            // Fetch the real orders from DB to avoid EF issues
            var orderIds = run.Orders.Select(o => o.OrderId).ToList();
            var realOrders = _context.Orders
                                     .Where(o => orderIds.Contains(o.OrderId))
                                     .ToList();

            if (!realOrders.Any())
                throw new Exception("No valid orders found for this run");

            run.Orders = realOrders;
            run.StartAt = DateTime.Now;

            _context.DeliveryRuns.Add(run);

            // Update each order
            foreach (var order in realOrders)
            {
                order.Status = "OnDelivery";
                order.RunId = run.RunId;
                _context.Orders.Update(order);
            }

            _context.SaveChanges();
        }

        // ==============================
        // Complete a Delivery Run
        // ==============================
        public void CompleteRun(int runId)
        {
            var run = _context.DeliveryRuns
                              .Include(r => r.Orders)
                              .FirstOrDefault(r => r.RunId == runId);

            if (run == null)
                throw new Exception("DeliveryRun not found");

            run.EndAt = DateTime.Now;

            foreach (var order in run.Orders)
            {
                order.Status = "Delivered";
                _context.Orders.Update(order);
            }

            _context.DeliveryRuns.Update(run);
            _context.SaveChanges();
        }

        // ==============================
        // Get all active runs
        // ==============================
        public IEnumerable<DeliveryRun> GetActiveRuns()
        {
            return _context.DeliveryRuns
                           .Include(r => r.Orders)
                           .Where(r => r.EndAt == null)
                           .ToList();
        }

        // ==============================
        // Add an order to an existing run
        // ==============================
        public void AddOrderToRun(int runId, int orderId)
        {
            var run = _context.DeliveryRuns
                              .Include(r => r.Orders)
                              .FirstOrDefault(r => r.RunId == runId);
            var order = _context.Orders.Find(orderId);

            if (run == null || order == null)
                throw new Exception("Run or Order not found");

            if (order.Status != "Pending")
                throw new Exception("Only pending orders can be added to a run");

            if (run.Orders.Any(o => o.OrderId == orderId))
                throw new Exception("Order already exists in this run");

            run.Orders.Add(order);
            order.Status = "OnDelivery";

            _context.SaveChanges();
        }

        // ==============================
        // Check if all orders in run are confirmed via QR
        // ==============================
        public bool AllOrdersConfirmed(int runId)
        {
            if (runId <= 0)
                throw new ArgumentException("Invalid RunId");

            var run = _context.DeliveryRuns
                              .Include(r => r.QrConfirmations)
                              .Include(r => r.Orders)
                              .FirstOrDefault(r => r.RunId == runId);

            if (run == null)
                throw new Exception("Run not found");

            return run.QrConfirmations.Count == run.Orders.Count;
        }
    }
}

