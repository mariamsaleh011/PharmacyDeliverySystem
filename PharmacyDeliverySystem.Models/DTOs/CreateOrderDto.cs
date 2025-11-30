using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{
    public class CartItemDto
    {
        public string Name { get; set; } = string.Empty;   // اسم المنتج من الكارت
        public decimal Price { get; set; }                 // السعر للوحدة
        public int Qty { get; set; }                       // الكمية
    }

    public class CreateOrderDto
    {
        public int? CustomerId { get; set; }               // لو لسه مفيش Customer سيبيه null

        // ✅ ندي قيمة افتراضية للـ List عشان ما تبقاش null
        public List<CartItemDto> Items { get; set; } = new();
    }
}
