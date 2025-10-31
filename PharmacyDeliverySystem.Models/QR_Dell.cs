using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class QR_Dell
{
    public int QR_id { get; set; }

    public int RunId { get; set; }

    public virtual QRConfirmation QR { get; set; } = null!;

    public virtual DeliveryRun Run { get; set; } = null!;
}
