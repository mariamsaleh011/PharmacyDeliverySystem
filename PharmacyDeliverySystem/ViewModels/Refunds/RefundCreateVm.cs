using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Refunds
{
    public class RefundCreateVm
    {
        [Required, StringLength(255)]
        public string PayId { get; set; } = null!;

        [StringLength(200)]
        public string? Reason { get; set; }
    }
}
