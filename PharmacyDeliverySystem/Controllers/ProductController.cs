using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers;

public class ProductController : Controller
{
    private readonly IProductManager  productManager;

    public ProductController(IProductManager  productManager)
    {
        this.productManager = productManager;
    }

    public IActionResult Index()
    {
        var products = productManager.GetAllProducts();
        return View(products);
    }

    public IActionResult Details(int id)
    {
        var product = productManager.GetProductById(id);
        return View(product);
    }
    //empty form
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(PRODUCT product)
    {
        if (ModelState.IsValid)
        {
            productManager.AddProduct(product);
            return RedirectToAction("Index");
        }

        return View(product);
    }

   
    //exiting data_form
    public IActionResult Edit(int id)
    {
        var product = productManager.GetProductById(id);
        return View(product);
    }
    [HttpPost]
    public IActionResult Edit(PRODUCT product)
    {
         if (ModelState.IsValid) 
         {
            productManager.UpdateProduct(product);
            return RedirectToAction("Index");
         }
         return View(product);
    }
    //confirmation page to the user before actually deleting.
    public IActionResult Delete(int id)
    {
        var product = productManager.GetProductById(id);
        return View(product);
    }
    [HttpPost, ActionName("DeleteProduct")]
    public IActionResult DeleteConfirmed(int id)
    {
        productManager.DeleteProduct(id);
        return RedirectToAction("Index");
    }
}