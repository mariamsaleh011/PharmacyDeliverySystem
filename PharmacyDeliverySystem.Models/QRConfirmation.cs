using System;

namespace PharmacyDeliverySystem.Models
{
    public partial class QrConfirmation
    {
        public int QR_Id { get; set; }
        public string? Code { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EXP_date { get; set; }
        public int? CustomerId { get; set; }
        public int? DeliveryRunId { get; set; }
        public int? OrderId { get; set; }
        public bool IsScanned { get; set; } = false;
        public DateTime? ScannedAt { get; set; }
        public string? ScannedBy { get; set; }

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual DeliveryRun? DeliveryRun { get; set; }
        public virtual Order? Order { get; set; }
    }
}
