using PharmacyDeliverySystem.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace PharmacyDeliverySystem.Models
{
    public class PharmacyAdmin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        public int PharmId { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("PharmId")]
        public virtual Pharmacy Pharmacy { get; set; } = null!;
    }
}
