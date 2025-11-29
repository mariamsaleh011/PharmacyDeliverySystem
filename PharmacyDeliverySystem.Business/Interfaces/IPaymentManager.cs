using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Business
{
    public interface IPaymentManager
    {
        IEnumerable<Payment> GetAllPayments();
        Payment? GetPaymentById(string payId);
        IEnumerable<Payment> GetPaymentsByStatus(string status);
        void CreatePayment(Payment payment);
        void UpdatePayment(Payment payment);
        void DeletePayment(string payId);
    }
}
