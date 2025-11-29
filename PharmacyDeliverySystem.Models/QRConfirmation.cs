using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models
{
    public partial class QrConfirmation
    {
        public int QR_Id { get; set; }
        public string? Code { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int? CustomerId { get; set; }

        public int? DeliveryRunId { get; set; }

        public DateTime? EXP_date { get; set; }
        public string? ScannedBy { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual DeliveryRun? DeliveryRun { get; set; }
    }
}
