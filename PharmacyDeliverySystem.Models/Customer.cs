using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public virtual ICollection<QrConfirmation> QrConfirmations { get; set; } = new List<QrConfirmation>();
    }
}