using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;


namespace PharmacyDeliverySystem.Business.Managers
{

    public class QrConfirmationManager : IQrConfirmationManager
    {
        private readonly PharmacyDeliveryContext _context;

        public QrConfirmationManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        // إنشاء QR confirmation جديد للـ Customer في Run معين
        public QrConfirmation CreateQrForCustomer(int customerId, int runId)
        {
            var customer = _context.Customers.Find(customerId);
            var run = _context.DeliveryRuns.Find(runId);

            if (customer == null || run == null)
                throw new Exception("Customer or DeliveryRun not found");

            var qr = new QrConfirmation
            {
                CustomerId = customerId,
                DeliveryRunId = runId,
                CreatedAt = DateTime.Now,
                Code = Guid.NewGuid().ToString().Substring(0, 8) // QR code random
            };

            _context.QrConfirmations.Add(qr);
            _context.SaveChanges();
            return qr;
        }

        // تسجيل مسح الـ QR من العميل
        public void ScanQr(int qrId, string scannedBy)
        {
            var qr = _context.QrConfirmations.Find(qrId);
            if (qr == null)
                throw new Exception("QR not found");

            qr.ScannedBy = scannedBy;
            _context.SaveChanges();
        }

        // التحقق إذا كل الأوردرات في Run تم عمل Scan لها
        public bool AllQrScanned(int runId)
        {
            var run = _context.DeliveryRuns.Find(runId);
            if (run == null)
                throw new Exception("Run not found");

            return run.QrConfirmations.All(q => !string.IsNullOrEmpty(q.ScannedBy));
        }

        // جلب جميع الـ QR confirmations لعميل معين
        public IEnumerable<QrConfirmation> GetQrByCustomer(int customerId)
        {
            return _context.QrConfirmations
                           .Where(q => q.CustomerId == customerId)
                           .ToList();
        }
    }
}
