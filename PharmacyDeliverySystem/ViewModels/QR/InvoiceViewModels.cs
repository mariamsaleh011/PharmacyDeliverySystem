using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.ViewModels
{
    public class InvoiceViewModel
    {
        public Order Order { get; set; }
        public QrConfirmation Qr { get; set; } // Only from QrConfirmation
    }
}
