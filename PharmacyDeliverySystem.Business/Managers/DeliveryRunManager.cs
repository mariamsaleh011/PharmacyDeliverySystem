using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PharmacyDeliverySystem.Business.Managers
{
    public class DeliveryRunManager : IDeliveryRunManager
    {
        private readonly PharmacyDeliveryContext _context;

        public DeliveryRunManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // إنشاء DeliveryRun جديد
        public void CreateRun(DeliveryRun run)
        {
            if (run == null)
                throw new ArgumentNullException(nameof(run), "DeliveryRun object cannot be null");


            if (run.RiderId <= 0)
                throw new ArgumentException("RiderId must be valid");

            if(run.Orders == null || !run.Orders.Any())
                throw new ArgumentException("DeliveryRun must have at least one order");

            run.StartAt = DateTime.Now;
            _context.DeliveryRuns.Add(run);
            _context.SaveChanges();
        }

        // إنهاء Runذ
        public void CompleteRun(int runId)
        {
            var run = _context.DeliveryRuns
                              .Where(r => r.RunId == runId)
                              .FirstOrDefault();
            if (run == null)
                throw new Exception("DeliveryRun not found");

            run.EndAt = DateTime.Now;

            // تحديث حالة كل الأوردرات المرتبطة
            foreach (var order in run.Orders)
            {
                order.Status = "Delivered";
            }

            _context.SaveChanges();
        }

        // جلب جميع الـ runs الجارية
        public IEnumerable<DeliveryRun> GetActiveRuns()
        {
            return _context.DeliveryRuns
                           .Where(r => r.EndAt == null)
                           .ToList();
        }

        // إضافة Order للـ run
        public void AddOrderToRun(int runId, int orderId)
        {
            var run = _context.DeliveryRuns.Find(runId);
            var order = _context.Orders.Find(orderId);

            if (run == null || order == null)
                throw new Exception("Run or Order not found");

            if (order.Status != "Pending")
                throw new Exception("Only pending orders can be added to a run");
            
            if (run.Orders.Any(o => o.OrderId == orderId))
                throw new Exception("Order already exists in this run");

            run.Orders.Add(order);
            order.Status = "OnDelivery"; // تحديث حالة الأوردر
            _context.SaveChanges();
        }

        // التحقق من تأكيد الـ QR لكل أوردر
        public bool AllOrdersConfirmed(int runId)
        {
            if (runId <= 0)
                throw new ArgumentException("Invalid RunId");

            var run = _context.DeliveryRuns.Find(runId);
            if (run == null)
                throw new Exception("Run not found");

            return run.QrConfirmations.Count == run.Orders.Count;
        }
    }
}
