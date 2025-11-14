using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PharmacyDeliverySystem.ViewModels.Prescriptions
{
    public class PrescriptionUploadVm : IValidatableObject
    {
        [Required, Range(1, int.MaxValue)]
        public int CustomerID { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int PharmId { get; set; }

        [StringLength(120)]
        public string? Name { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;

        public int? OrderID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (File == null || File.Length == 0)
                yield return new ValidationResult("File is required", new[] { nameof(File) });

            if (File != null)
            {
                var allowed = new[] { "image/jpeg", "image/png", "application/pdf" };
                if (!allowed.Contains(File.ContentType))
                    yield return new ValidationResult("Allowed types: jpg, png, pdf", new[] { nameof(File) });

                const long max = 5 * 1024 * 1024; // 5MB
                if (File.Length > max)
                    yield return new ValidationResult("Max size is 5MB", new[] { nameof(File) });
            }
        }
    }
}
