using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class PharmacyChat
{
    public int PharmacyChatId { get; set; }

    public int ChatId { get; set; }

    public int PharmId { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual Pharmacy Pharm { get; set; } = null!;
}
