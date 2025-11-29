using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Pharmacy
    {
        public int PharmId { get; set; }

        public string LicenceNo { get; set; } = null!;

        public string? TaxId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}