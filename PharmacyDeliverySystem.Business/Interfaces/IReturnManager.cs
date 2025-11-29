using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IReturnManager
    {
        IEnumerable<Returnn> GetAll();
        Returnn? GetById(int id);
        IEnumerable<Returnn> GetByOrder(int orderId);

        void Add(Returnn entity);
        void Update(Returnn entity);
        void SetStatus(int id, string status);
        void Delete(int id);
    }
}
