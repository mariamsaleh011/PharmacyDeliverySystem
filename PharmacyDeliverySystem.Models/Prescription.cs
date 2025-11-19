using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Prescription
    {
        public int PreId { get; set; }

        public string Status { get; set; } = null!;

        public string Image { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int CustomerId { get; set; }

        public int PharmId { get; set; }

        public int? OrderId { get; set; }

        public virtual Customer Customer { get; set; } = null!;

        public virtual Order? Order { get; set; }

        public virtual Pharmacy Pharm { get; set; } = null!;
    }
}