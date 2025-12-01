using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.Models.Validation;
using PharmacyDeliverySystem.ViewModels.Prescriptions;

namespace PharmacyDeliverySystem.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionManager _manager;
        private readonly IWebHostEnvironment _env;

        public PrescriptionController(IPrescriptionManager manager, IWebHostEnvironment env)
        {
            _manager = manager;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_manager.GetAll());
        }

        public IActionResult Details(int id)
        {
            var item = _manager.GetById(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new PrescriptionUploadVm());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(PrescriptionUploadVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Create upload folder
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Generate file name
            var ext = Path.GetExtension(vm.File.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            // Save file to server
            using (var stream = System.IO.File.Create(fullPath))
            {
                await vm.File.CopyToAsync(stream);
            }

            // Create entity
            var entity = new Prescription
            {
                CustomerId = vm.CustomerID,
                PharmId = vm.PharmId,
                Name = vm.Name ?? string.Empty,   // ✅ لو الـ VM ممكن يكون فيه null
                OrderId = vm.OrderID,
                Image = $"/uploads/{fileName}",
                Status = PrescriptionStatuses.Uploaded
            };

            // Save to DB
            _manager.Add(entity);

            return RedirectToAction(nameof(Details), new { id = entity.PreId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _manager.GetById(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Prescription model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _manager.Update(model);

            return RedirectToAction(nameof(Details), new { id = model.PreId });
        }

        [HttpPost]
        public IActionResult SetStatus(int id, string status)
        {
            _manager.SetStatus(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _manager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
