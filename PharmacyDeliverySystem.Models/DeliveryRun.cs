﻿using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class DeliveryRun
{
    public int RunId { get; set; }

    public int RiderId { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<QR_Dell> QR_Dells { get; set; } = new List<QR_Dell>();
}
