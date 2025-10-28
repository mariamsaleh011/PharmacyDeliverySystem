using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class OrderItem
{
    public int? ProId { get; set; }

    public DateTime? TimeId { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PRODUCT? Pro { get; set; }
}
