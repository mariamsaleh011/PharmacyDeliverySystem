using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models { 

public partial class Product
{
    public int ProId { get; set; }

    public string Name { get; set; } = null!;

    public string? Barcode { get; set; }

    public string? Brand { get; set; }

    public string? VatRate { get; set; }

    public string? Dosage { get; set; }

    public int? PharmId { get; set; }

    public virtual Pharmacy? Pharm { get; set; }
}
}