using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IWebHostEnvironment _env;

        public AdminController(IProductManager productManager, IWebHostEnvironment env)
        {
            _productManager = productManager;
            _env = env;
        }

        // لوحة إدارة المنتجات – للصيدلي فقط
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Admin()
        {
            // نستقبل أي رسالة Debug من Create
            ViewBag.Debug = TempData["DebugMessage"];

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
                    p.Barcode,
                    p.Brand,
                    p.VatRate,
                    p.Dosage,
                    p.DrugType,
                    p.PharmId,
                    IsActive = p.Quantity > 0
                })
                .ToList<object>();

            return View(products);
        }

        // عرض كل المنتجات (مفتوحة للجميع)
        public IActionResult Index()
        {
            var products = _productManager.GetAll();
            return View(products);
        }

        // تفاصيل منتج واحد (مفتوحة للجميع)
        public IActionResult Details(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // إنشاء منتج جديد – للصيدلي فقط
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Create(Product product, IFormFile? ImageFile)
        {
            // بس علشان نتاكد إننا دخلنا الاكشن
            TempData["DebugMessage"] = "Reached Create. Name = " + (product?.Name ?? "NULL");

            // ✅ خلي الـ ProId صفر دايمًا في الإضافة (EF هيولّد ID جديد)
            product.ProId = 0;

            // ✅ شيل أي Error من الـ ModelState خاص بـ ProId (لأن الفورم بيبعته فاضي "")
            ModelState.Remove(nameof(Product.ProId));

            // لو لسه فيه أخطاء في فيلدات تانية نطبعها
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState
                        .Where(kvp => kvp.Value.Errors.Count > 0)
                        .Select(kvp => kvp.Key + ": " +
                            string.Join(",", kvp.Value.Errors.Select(e => e.ErrorMessage))));

                TempData["DebugMessage"] = "ModelState invalid -> " + errors;
                return RedirectToAction(nameof(Admin));
            }

            // حفظ الصورة لو موجودة
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

            try
            {
                _productManager.Add(product);
                TempData["DebugMessage"] = "Product saved successfully. Name = " +
                                           (product.Name ?? "NULL") +
                                           ", Price = " + product.Price;
            }
            catch (Exception ex)
            {
                TempData["DebugMessage"] = "Exception while saving product: " + ex.Message;
            }

            return RedirectToAction(nameof(Admin));
        }

        // تعديل منتج – للصيدلي فقط
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Edit(Product product, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Admin));

            var existing = _productManager.GetById(product.ProId);
            if (existing == null)
                return NotFound();

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

        // صفحة تأكيد الحذف – للصيدلي فقط
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Delete(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // تنفيذ الحذف – للصيدلي فقط
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pharmacy")]
        public IActionResult Delete(int id, string? returnUrl = null)
        {
            _productManager.Delete(id);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Admin));
        }
    }
}
