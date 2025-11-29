using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Returnn
{
    public int ReturnId { get; set; }

    public string? Status { get; set; }

    public string? Reason { get; set; }

    public int? OrderID { get; set; }

    public virtual Order? Order { get; set; }
}
