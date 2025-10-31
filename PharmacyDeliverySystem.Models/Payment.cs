using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Payment
{
    public int PayId { get; set; }

    public string status { get; set; } = null!;

    public string METHOD { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual ICollection<Invoice> Invs { get; set; } = new List<Invoice>();
}
