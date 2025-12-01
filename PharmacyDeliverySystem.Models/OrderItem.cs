using System;

namespace PharmacyDeliverySystem.Models
{
    public partial class OrderItem
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }   // ده هو الـ Unit Price

        public virtual Product Product { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;   // 👈 بدل ICollection<OrderItem>
    }
}
