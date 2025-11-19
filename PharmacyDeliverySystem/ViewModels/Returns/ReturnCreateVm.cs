using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Returns
{
    public class ReturnCreateVm
    {
        [Required, Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Required, StringLength(200)]
        public string Reason { get; set; } = null!;

        [Required]
        [RegularExpression("Requested|Approved|Rejected|Completed",
            ErrorMessage = "Status must be: Requested, Approved, Rejected, or Completed.")]
        public string Status { get; set; } = "Requested";
    }
}
