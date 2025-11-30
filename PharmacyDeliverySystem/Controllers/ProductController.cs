using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using System.IO;

namespace PharmacyDeliverySystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductManager productManager, IWebHostEnvironment env)
        {
            _productManager = productManager;
            _env = env;
        }

        // ================================
        // لوحة إدارة المنتجات (الـ UI الجديد)
        // GET: /Product/Admin
        // ================================
        public IActionResult Admin()
        {
            // بنجهز موديل ديناميكي فيه IsActive محسوبة من الـ Quantity
            var products = _productManager.GetAll()
                .Select(p => new
                {
                    Id = p.ProId,
                    p.Name,
                    p.Description,
                    p.ImageUrl,
                    p.Price,
                    p.OldPrice,
                    p.Quantity,
                    IsActive = p.Quantity > 0     // نشط لو الكمية > 0
                })
                .ToList<object>();

            // Admin.cshtml مكتوبة @model IEnumerable<dynamic> فده هيشتغل عادي
            return View(products);
        }

        // لو محتاجين الـ Index/Details القديمة
        public IActionResult Index()
        {
            var products = _productManager.GetAll();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // ================================
        // CREATE من المودال
        // POST: /Product/Create
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Admin));

            // حفظ الصورة
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fs);
                }

                product.ImageUrl = "/images/products/" + fileName;
            }

            _productManager.Add(product);
            return RedirectToAction(nameof(Admin));
        }

        // ================================
        // EDIT من المودال
        // POST: /Product/Edit
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Admin));

            var existing = _productManager.GetById(product.ProId);
            if (existing == null)
                return NotFound();

            // تحديث الخصائص الموجودة فعلاً في الـ Product
            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.OldPrice = product.OldPrice;
            existing.Quantity = product.Quantity;
            existing.Description = product.Description;
            existing.Barcode = product.Barcode;
            existing.Brand = product.Brand;
            existing.VatRate = product.VatRate;
            existing.Dosage = product.Dosage;
            existing.DrugType = product.DrugType;
            existing.PharmId = product.PharmId;

            // صورة جديدة لو اتبعتت
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fs);
                }

                existing.ImageUrl = "/images/products/" + fileName;
            }

            _productManager.Update(existing);
            return RedirectToAction(nameof(Admin));
        }

        // ================================
        // DELETE (لو عندك صفحة تأكيد قديمة)
        // ================================
        public IActionResult Delete(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // ================================
        // POST: /Product/Delete
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string? returnUrl = null)
        {
            _productManager.Delete(id);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Admin));
        }
    }
}
