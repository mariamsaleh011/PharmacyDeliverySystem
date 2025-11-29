using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly PharmacyDeliveryContext _context;

        public PaymentManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            return _context.Payments.ToList();
        }

        public Payment? GetPaymentById(string payId)
        {
            return _context.Payments.FirstOrDefault(p => p.PayId == payId);
        }

        public IEnumerable<Payment> GetPaymentsByStatus(string status)
        {
            return _context.Payments.Where(p => p.Status == status).ToList();
        }

        public void CreatePayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        public void UpdatePayment(Payment payment)
        {
            _context.Payments.Update(payment);
            _context.SaveChanges();
        }

        public void DeletePayment(string payId)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.PayId == payId);
            if (payment == null) return;
            _context.Payments.Remove(payment);
            _context.SaveChanges();
        }
    }
}
