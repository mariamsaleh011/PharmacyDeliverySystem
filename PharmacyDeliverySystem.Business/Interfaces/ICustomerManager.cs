using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business;

public interface ICustomerManager
{
    IEnumerable<Customer> GetAllCustomers();
    Customer? GetCustomerById(int id);
    void AddCustomer(Customer customer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(int id);
}