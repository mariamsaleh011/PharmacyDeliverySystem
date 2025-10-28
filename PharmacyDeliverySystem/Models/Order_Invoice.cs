using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Order_Invoice
{
    public int OrderID { get; set; }

    public int? InvId { get; set; }

    public virtual Invoice? Inv { get; set; }

    public virtual Order Order { get; set; } = null!;
}
