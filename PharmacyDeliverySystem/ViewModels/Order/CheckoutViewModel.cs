using System.Collections.Generic;

namespace PharmacyDeliverySystem.ViewModels.Order
{
    public class CheckoutItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CheckoutViewModel
    {
        // نجعل CustomerId اختياري لأن الفارمسي بس اللي يستخدمه
        public int? CustomerId { get; set; }

        public List<CheckoutItemViewModel> Items { get; set; } = new();
    }
}
