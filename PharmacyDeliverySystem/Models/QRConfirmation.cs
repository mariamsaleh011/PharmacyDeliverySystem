using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class QRConfirmation
{
    public int QR_Id { get; set; }

    public string? Code { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? EXP_date { get; set; }

    public string? ScannedBy { get; set; }

    public int? CustomerID { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual QR_Dell? QR_Dell { get; set; }
}
