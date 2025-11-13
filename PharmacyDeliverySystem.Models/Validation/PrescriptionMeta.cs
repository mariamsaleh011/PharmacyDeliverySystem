using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyDeliverySystem.Models.Validation
{
    public static class PrescriptionStatuses
    {
        public const string Uploaded = "Uploaded";
        public const string UnderReview = "UnderReview";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Fulfilled = "Fulfilled";
    }

    public class PrescriptionMeta
    {
        [Key]
        public int PreId { get; set; }

        [Required]
        [RegularExpression("Uploaded|UnderReview|Approved|Rejected|Fulfilled",
            ErrorMessage = "Invalid status")]
        public string Status { get; set; } = PrescriptionStatuses.Uploaded;

        [Required, StringLength(255)]
        public string Image { get; set; } = null!; // path/url

        [StringLength(120)]
        public string? Name { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int CustomerID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int PharmId { get; set; }

        public int? OrderID { get; set; }
    }

	//connect the Prescription class with this class
	[ModelMetadataType(typeof(PrescriptionMeta))]
    public partial class Prescription { }
}
