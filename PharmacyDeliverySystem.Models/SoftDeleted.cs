using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyDeliverySystem.Models
{
    [Table("SoftDeleted")]   // ⬅️ اسم الجدول في SQL
    public class SoftDeleted
    {
        [Key]
        public int ProId { get; set; }

        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? Brand { get; set; }

        [Column("VAT_Rate")]
        public string? VAT_Rate { get; set; }

        public string? Dosage { get; set; }
        public int? PharmId { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? DrugType { get; set; }
        public int? Quantity { get; set; }

        [Column("deleted_at")]
        public DateTime DeletedAt { get; set; }
    }
}
