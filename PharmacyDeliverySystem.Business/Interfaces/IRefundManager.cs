using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IRefundManager
    {
        IEnumerable<Refund> GetAll();
        Refund? GetById(int id);
        IEnumerable<Refund> GetByPayment(string payId);

        void Add(Refund entity);
        void Update(Refund entity);
        void Delete(int id);
    }
}
