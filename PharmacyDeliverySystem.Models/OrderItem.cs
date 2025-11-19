using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class OrderItem
    {
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int? Quantity { get; set; }

        public string? Status { get; set; }

        public decimal? Price { get; set; }
    }
}
