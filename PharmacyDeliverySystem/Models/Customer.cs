using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Customer
{
    public int CustomerID { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<CustomerChat> CustomerChats { get; set; } = new List<CustomerChat>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<QRConfirmation> QRConfirmations { get; set; } = new List<QRConfirmation>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
