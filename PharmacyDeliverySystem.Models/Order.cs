using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public decimal? Price { get; set; }

    public string? Quantity { get; set; }

    public string? Status { get; set; }

    public int? PharmId { get; set; }

    public int? RunId { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Order_Invoice? Order_Invoice { get; set; }

    public virtual Pharmacy? Pharm { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<Returnn> Returnns { get; set; } = new List<Returnn>();

    public virtual DeliveryRun? Run { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
