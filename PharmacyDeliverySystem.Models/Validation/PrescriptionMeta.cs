using System.ComponentModel.DataAnnotations;

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

    // هذا مجرد Metadata class — NOT linked by ModelMetadataType
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

    // ❌ تم إزالة ModelMetadataType لأنه يسبب كسر في الـBuild داخل Models
    // ❌ لا نربط PrescriptionMeta بـ Prescription داخل مشروع Models
    // لو محتاج تربطهم، هنعمله داخل الـWeb Project فقط (ViewModels أو Metadata DI)
}
