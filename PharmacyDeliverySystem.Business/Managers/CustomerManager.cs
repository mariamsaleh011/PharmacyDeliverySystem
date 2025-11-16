using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business;

public class CustomerManager : ICustomerManager
{
    private readonly PharmacyDeliveryContext context;

    public CustomerManager(PharmacyDeliveryContext context)
    {
        this.context = context;
    }
    public IEnumerable<Customer> GetAllCustomers()
    {
        return this.context.Customers.ToList();
    }

    public Customer? GetCustomerById(int id)
    {
        return this.context.Customers.Find(id);
    }

    public void AddCustomer(Customer customer)
    {
        this.context.Customers.Add(customer);
    }

    public void UpdateCustomer(Customer customer)
    {
        this.context.Customers.Update(customer);
        this.context.SaveChanges();
    }

    public void DeleteCustomer(int id)
    {
        this.context.Customers.Remove(this.context.Customers.Find(id));
        this.context.SaveChanges();
    }
}