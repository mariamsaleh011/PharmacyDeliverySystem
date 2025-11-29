using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels
{
    public class PharmacyRegisterViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(50)]
        public string LicenceNo { get; set; } = null!;

        [Required, StringLength(50)]
        public string TaxId { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be at least 8 chars, with upper, lower and digit")]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class PharmacyLoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
