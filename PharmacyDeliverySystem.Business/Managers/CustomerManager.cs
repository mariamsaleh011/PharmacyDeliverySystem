using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Business.Interfaces;

namespace PharmacyDeliverySystem.Business.Managers
{
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
            var customer = this.context.Customers.Find(id);
            if (customer is null)
            {
                // مفيش عميل بالـ id ده، مفيش حاجة أعملها
                return;
            }

            this.context.Customers.Remove(customer);
            this.context.SaveChanges();
        }
    }
}
