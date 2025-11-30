using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.ViewModels
{
    public class InvoiceViewModel
    {
        public PharmacyDeliverySystem.Models.Order Order { get; set; } = null!;
        public QrConfirmation Qr { get; set; } = null!; // 👈 ضيف = null!
    }
}
