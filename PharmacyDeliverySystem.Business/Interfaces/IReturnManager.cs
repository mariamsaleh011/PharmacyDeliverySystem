using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IReturnManager
    {
        IEnumerable<Return> GetAll();
        Return? GetById(int id);
        IEnumerable<Return> GetByOrder(int orderId);

        void Add(Return entity);
        void Update(Return entity);
        void SetStatus(int id, string status);
        void Delete(int id);
    }
}
