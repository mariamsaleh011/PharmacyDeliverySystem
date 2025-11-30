using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class QrConfirmationManager : IQrConfirmationManager
    {
        private readonly PharmacyDeliveryContext _context;

        public QrConfirmationManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public QrConfirmation CreateQrForCustomer(int customerId, int runId)
        {
            var run = _context.DeliveryRuns.Find(runId);
            if (run == null) throw new Exception("Run not found");

            var qr = new QrConfirmation
            {
                CustomerId = customerId,
                DeliveryRunId = runId,
                CreatedAt = DateTime.Now,
                Code = Guid.NewGuid().ToString().Substring(0, 8)
            };

            _context.QrConfirmations.Add(qr);
            _context.SaveChanges();
            return qr;
        }

        public QrConfirmation CreateQrForOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) throw new Exception("Order not found");

            var qr = new QrConfirmation
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                DeliveryRunId = order.RunId,
                CreatedAt = DateTime.Now,
                Code = Guid.NewGuid().ToString().Substring(0, 8)
            };

            _context.QrConfirmations.Add(qr);
            _context.SaveChanges();

            return qr;
        }

        public void ScanQr(int qrId, string scannedBy)
        {
            var qr = _context.QrConfirmations.Find(qrId);
            if (qr == null) throw new Exception("QR not found");

            qr.IsScanned = true;
            qr.ScannedAt = DateTime.Now;
            qr.ScannedBy = scannedBy;

            if (qr.OrderId.HasValue)
            {
                var order = _context.Orders.Find(qr.OrderId.Value);
                if (order != null)
                    order.Status = "Delivered";
            }

            _context.SaveChanges();
        }

        public QrConfirmation GetQrById(int qrId)
        {
            var qr = _context.QrConfirmations
                             .Include(q => q.Order)
                             .FirstOrDefault(q => q.QR_Id == qrId);

            if (qr is null)
                throw new Exception("QR not found");

            return qr;
        }

        public QrConfirmation GetQrByOrder(int orderId)
        {
            var qr = _context.QrConfirmations
                             .Include(q => q.Order)
                             .FirstOrDefault(q => q.OrderId == orderId);

            if (qr is null)
                throw new Exception("QR not found");

            return qr;
        }

        public bool AllOrdersConfirmed(int runId)
        {
            var run = _context.DeliveryRuns
                              .Include(r => r.Orders)
                              .FirstOrDefault(r => r.RunId == runId);

            if (run == null) throw new Exception("Run not found");

            return run.Orders.All(o => o.Status == "Delivered");
        }

        public void CompleteRun(int runId)
        {
            var run = _context.DeliveryRuns
                              .Include(r => r.Orders)
                              .FirstOrDefault(r => r.RunId == runId);
            if (run == null) throw new Exception("Run not found");

            run.EndAt = DateTime.Now;
            foreach (var order in run.Orders)
            {
                order.Status = "Delivered";
            }

            _context.SaveChanges();
        }

        public IEnumerable<QrConfirmation> GetQrByCustomer(int customerId)
        {
            return _context.QrConfirmations
                           .Include(q => q.Order)
                           .Where(q => q.CustomerId == customerId)
                           .ToList();
        }
    }
}
