using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly PharmacyDeliveryContext _context;

        public ProductManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
            => _context.Products.AsNoTracking().ToList();

        public Product? GetById(int id)
            => _context.Products.Find(id);

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Products.Find(id);
            if (entity == null) return;

            _context.Products.Remove(entity);
            _context.SaveChanges();
        }
    }
}
