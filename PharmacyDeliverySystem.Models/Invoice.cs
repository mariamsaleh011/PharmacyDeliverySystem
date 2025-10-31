using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Invoice
{
    public int InvId { get; set; }

    public int? PayId { get; set; }

    public virtual Order_Invoice? Order_Invoice { get; set; }

    public virtual Payment? Pay { get; set; }

    public virtual ICollection<Payment> Pays { get; set; } = new List<Payment>();
}
