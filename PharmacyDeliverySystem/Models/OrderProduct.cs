using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class OrderProduct
{
    public int ProId { get; set; }

    public int OrderID { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PRODUCT Pro { get; set; } = null!;
}
