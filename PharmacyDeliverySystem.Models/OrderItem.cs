using System;

namespace PharmacyDeliverySystem.Models
{
    public partial class OrderItem
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }

        // ✅ EF هيملّاها من الداتابيز، وإحنا بنقول للمترجم "متقلقش"
        public virtual Product Product { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
