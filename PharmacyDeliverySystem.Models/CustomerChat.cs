using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class CustomerChat
{
    public int CustomerChatId { get; set; }

    public int ChatId { get; set; }

    public int CustomerID { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
