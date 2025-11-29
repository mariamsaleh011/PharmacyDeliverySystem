// ملف: PharmacyDeliverySystem.Business.Managers/ProductManager.cs

using System.Collections.Generic;
using System.Linq;
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
        {
            // 💡 تحسين: إذا كان لديك علاقة (Relationship) مع Pharmacy، استخدم Include لجلبها مع المنتجات.
            // إذا لم يكن هناك حاجة لـ Pharmacy في هذا السياق، يمكن تركها كما هي.
            return _context.Products
                  // .Include(p => p.Pharm) // أضف هذا إذا كنت تحتاج بيانات الصيدلية
                  .AsNoTracking()
                  .ToList();
        }

        public Product? GetById(int id)
        {
            // 💡 تحسين: استخدام SingleOrDefault/FirstOrDefault أفضل من Find() 
            // إذا كنا نريد استخدام Includes في المستقبل. Find() يعمل فقط بالـ Primary Key.
            return _context.Products
                 // .Include(p => p.Pharm) // أضف هذا إذا كنت تحتاج بيانات الصيدلية
                 .AsNoTracking() // عادةً لا نحتاج التتبع في قراءة التفاصيل
                 .FirstOrDefault(p => p.ProId == id);
        }

        // ... دوال Add، Update، Delete تبقى كما هي لأنها تعمل على الكائن بالكامل

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