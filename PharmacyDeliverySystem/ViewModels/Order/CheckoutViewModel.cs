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
        // مش هنستخدمه من الـ JS دلوقت، الـ Controller هياخده من الـ Logged-in customer
        public int CustomerId { get; set; }
        public List<CheckoutItemViewModel> Items { get; set; } = new();
    }
}
