using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Refund
{
    public int RefId { get; set; }

    public int? Amount { get; set; }

    public string? Reason { get; set; }

    public int? PayId { get; set; }

    public virtual Payment? Pay { get; set; }
}
