using System;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.OrderItem
{
    public class OrderItemViewModels
    {
        public class OrderItemInputViewModel
        {
            [Required(ErrorMessage = "OrderId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "OrderId لازم يكون أكبر من صفر")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "ProductId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "ProductId لازم يكون أكبر من صفر")]
            public int ProductId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Quantity لازم يكون أكبر من صفر")]
            public int? Quantity { get; set; }

            [StringLength(50, ErrorMessage = "Status لا يمكن أن يزيد عن 50 حرف")]
            public string? Status { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Price لازم يكون أكبر من أو يساوي صفر")]
            public decimal? Price { get; set; }
        }

        public class OrderItemResponseViewModel
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int? Quantity { get; set; }
            public string? Status { get; set; }
            public decimal? Price { get; set; }
        }

        public class OrderItemUpdateQuantityViewModel
        {
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity لازم يكون أكبر من صفر")]
            public int Quantity { get; set; }
        }

        public class OrderItemUpdateStatusViewModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "Status لا يمكن أن يزيد عن 50 حرف")]
            public string Status { get; set; } = null!; // 👈 ضيف
        }
    }
}