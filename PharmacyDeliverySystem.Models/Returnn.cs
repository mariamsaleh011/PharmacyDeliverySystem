using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Return
    {
        public int ReturnId { get; set; }

        public string? Status { get; set; }

        public string? Reason { get; set; }

        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
