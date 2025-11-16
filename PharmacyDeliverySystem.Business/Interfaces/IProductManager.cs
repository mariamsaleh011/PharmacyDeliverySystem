using System.Collections.Generic;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
    public interface IProductManager
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}
