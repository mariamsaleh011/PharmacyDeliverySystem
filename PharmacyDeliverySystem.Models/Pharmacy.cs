using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Pharmacy
{
    public int PharmId { get; set; }

    public string LicenceNo { get; set; } = null!;

    public string? TaxId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PRODUCT> PRODUCTs { get; set; } = new List<PRODUCT>();

    public virtual ICollection<PharmacyChat> PharmacyChats { get; set; } = new List<PharmacyChat>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
