using PharmacyDeliverySystem.Models;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IPharmacyManager
    {
        IEnumerable<Pharmacy> GetAllPharmacies();
        Pharmacy? GetById(int id);
        IEnumerable<Pharmacy> GetByName(string name);
        void Create(Pharmacy pharmacy);
        void Update(Pharmacy pharmacy);
        void Delete(int id);
    }
}
