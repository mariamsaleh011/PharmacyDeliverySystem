using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyDeliverySystem.Models
{
    public partial class Product
    {
        public int ProId { get; set; }

        // من الجدول: Name (nvarchar(20), not null)
        public string Name { get; set; } = null!;

        // من الجدول: Barcode (nvarchar(40), null)
        public string? Barcode { get; set; }

        // من الجدول: Brand (nvarchar(20), null)
        public string? Brand { get; set; }

        // من الجدول: VAT_Rate (nvarchar(30), null)
        public string? VatRate { get; set; }

        // من الجدول: Dosage (nvarchar(40), null)
        public string? Dosage { get; set; }

        // من الجدول: PharmId (int, null, FK)
        public int? PharmId { get; set; }

        // من الجدول: Price (decimal(18,2), not null)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }   // بدل decimal? 

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        // من الجدول: ImageUrl (nvarchar(500), null)
        public string? ImageUrl { get; set; }

        // من الجدول: Description (nvarchar(max), null)
        public string? Description { get; set; }

        // من الجدول: DrugType (nvarchar(max), null)
        public string? DrugType { get; set; }

        // من الجدول: Quantity (int, not null)
        public int? Quantity { get; set; }

        // الـ navigation اللي كانت عندك
        public virtual Pharmacy? Pharm { get; set; }
    }
}
