using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;

        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        // GET: /Product
        public IActionResult Index()
        {
            var products = _productManager.GetAll();
            return View(products);
        }

        // GET: /Product/Details/5
        public IActionResult Details(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _productManager.Add(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _productManager.Update(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _productManager.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productManager.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
