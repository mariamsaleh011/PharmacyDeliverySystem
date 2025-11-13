using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business;


public interface IProductManager
{
    IEnumerable<PRODUCT> GetAllProducts();
     PRODUCT? GetProductById(int id);
     void AddProduct(PRODUCT product);
     void UpdateProduct(PRODUCT product);
     void DeleteProduct(int id);
     
}