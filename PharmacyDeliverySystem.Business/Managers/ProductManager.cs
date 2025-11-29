using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;              // ⭐ ضيف السطر ده
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
        {
            return _context.Products
                // .Include(p => p.Pharm)   // لو احتجت بيانات الصيدلية
                .AsNoTracking()
                .ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products
                // .Include(p => p.Pharm)
                .AsNoTracking()
                .FirstOrDefault(p => p.ProId == id);
        }

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
