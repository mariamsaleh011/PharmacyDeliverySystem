using Microsoft.AspNetCore.Mvc;
using PharmacyDeliverySystem.Business;          
using PharmacyDeliverySystem.Models;



namespace PharmacyDeliverySystem.Controllers;

public class OrderController : Controller
{
    private readonly IOrderManager  orderManager;

    public OrderController(IOrderManager orderManager)
    {
        this.orderManager = orderManager;
    }
    //GET : list of orders
    public IActionResult Index()
    {
        var orders = orderManager.GetAllOrders();
        return View(orders);
    }
    //GET : order details byId
    public IActionResult Details(int id)
    {
        var order = orderManager.GetOrderById(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
    //GET : order details byCustomer
    public IActionResult ByCustomer(int customerId)
    {
        var orders =  orderManager.GetOrdersByCustomer(customerId);
        return View("Index",orders);
    }
    //GET : order details byPharmacy
    public IActionResult ByPharmacy(int pharmacyId)
    {
        var orders =  orderManager.GetOrdersByPharmacy(pharmacyId);
        return View("Index",orders);
    }
    //GET : order details byStatus
    public IActionResult ByStatus(string status)
    {
        var orders =  orderManager.GetOrdersByStatus(status);
        return View("Index",orders);
    }
    //GET : show create form
    public IActionResult Create()
    {
        return View();
    }
    //POST : create new order
    [HttpPost]
    //security feature
    [ValidateAntiForgeryToken]
    public IActionResult Create(Order order)
    {
        if (ModelState.IsValid)
        {
            orderManager.CreateOrder(order);
            return RedirectToAction("Index");
        }
        return View(order);
    }
    // GET : show edit form
    public IActionResult Edit(int id)
    {
        var order = orderManager.GetOrderById(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
    [HttpPost] //POST : update order
    public IActionResult Edit(Order order)
    {
        if (ModelState.IsValid)
        {
            this.orderManager.UpdateOrder(order);
            return RedirectToAction("Index");
        }
        return View(order);
    }

    //GET : show confirmation page to the user before actually deleting.
    public IActionResult Delete(int id)
    {
        var order = orderManager.GetOrderById(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
    [HttpPost, ActionName("CancelOrder")] //POST : Actually deletes after confirmation
    public IActionResult DeleteConfirmed(int id)
    {
        orderManager.CancelOrder(id);
        return RedirectToAction("Index");
    }
    //Update order status
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateOrderStatus(int orderId, string newStatus)
    {
        orderManager.UpdateOrderStatus(orderId, newStatus);
        //new { id = orderId } tells MVC, "Set the id parameter in the URL to the value of orderId for this redirect."
        return RedirectToAction("Details", new { id = orderId });
    }
    
    //Assign order to delivery
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AssignOrderToDeliveryRun(int orderId, int runId)
    {
        orderManager.AssignOrderToDeliveryRun(orderId, runId);
    return RedirectToAction("Details", new { id = orderId });
    }
    
}