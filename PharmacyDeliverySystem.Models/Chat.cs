using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Chat
    {
        public int ChatId { get; set; }

        public string? Status { get; set; }

        public string? Channel { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? PharmacyId { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Order? Order { get; set; }

        public virtual Pharmacy? Pharmacy { get; set; }
    }
}