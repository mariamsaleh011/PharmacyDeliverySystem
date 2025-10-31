using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class PRODUCT
{
    public int ProId { get; set; }

    public string Name { get; set; } = null!;

    public string? Barcode { get; set; }

    public string? Brand { get; set; }

    public string? VAC_Rate { get; set; }

    public string? Dosage { get; set; }

    public int? PharmId { get; set; }

    public virtual OrderProduct? OrderProduct { get; set; }

    public virtual Pharmacy? Pharm { get; set; }
}
