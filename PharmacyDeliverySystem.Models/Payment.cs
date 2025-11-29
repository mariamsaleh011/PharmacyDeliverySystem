using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Payment
    {
        public string PayId { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Method { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    }
}