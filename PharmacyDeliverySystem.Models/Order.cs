using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{

    public partial class Order
    {
        public int OrderId { get; set; }

        public decimal? Price { get; set; }

        public string? Quantity { get; set; }

        public string? Status { get; set; }

        public int? PharmId { get; set; }

        public int? RunId { get; set; }

        public string? PdfUrl { get; set; }

        public int? InvoiceNo { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? PaymentId { get; set; }

        public int? CustomerId { get; set; }

        public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public virtual Customer? Customer { get; set; }

        public virtual Payment? Payment { get; set; }

        public virtual Pharmacy? Pharm { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public virtual DeliveryRun? Run { get; set; }   
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Return> Returns { get; set; } = new List<Return>();

    }
}