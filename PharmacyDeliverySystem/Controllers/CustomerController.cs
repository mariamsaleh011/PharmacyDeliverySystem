
using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerManager customerManager;

    public CustomerController(ICustomerManager customerManager)
    {
        this.customerManager = customerManager;
    }
    //GET : list all customers
    public IActionResult Index()
    {
        var customers = this.customerManager.GetAllCustomers();
        return View(customers);
    }
    //GET : show one customer details
    public IActionResult Details(int id)
    {
        var customer = this.customerManager.GetCustomerById(id);
        return View(customer);
    }
    //GET : show create form
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost] //POST : create new customer
    public IActionResult Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            this.customerManager.AddCustomer(customer);
            return RedirectToAction("Index");
        }
        return View(customer);
    }
    // GET : show edit form
    public IActionResult Edit(int id)
    {
        var customer = this.customerManager.GetCustomerById(id);
        return View(customer);
    }
    [HttpPost] //POST : update new customer
    public IActionResult Edit(Customer customer)
    {
        if (ModelState.IsValid)
        {
            this.customerManager.UpdateCustomer(customer);
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    //GET : show confirmation page to the user before actually deleting.
    public IActionResult Delete(int id)
    {
        var customer = customerManager.GetCustomerById(id);
        return View(customer);
    }
    [HttpPost, ActionName("DeleteCustomer")] //POST : Actually deletes after confirmation
    public IActionResult DeleteConfirmed(int id)
    {
        customerManager.DeleteCustomer(id);
        return RedirectToAction("Index");
    }
}