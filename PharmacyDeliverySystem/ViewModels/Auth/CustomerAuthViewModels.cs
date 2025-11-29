using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels
{
    public class CustomerRegisterViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(15)]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "Phone is not valid")]
        public string PhoneNumber { get; set; } = null!;

        [Required, StringLength(255)]
        public string Address { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be at least 8 chars, with upper, lower and digit")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class CustomerLoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
