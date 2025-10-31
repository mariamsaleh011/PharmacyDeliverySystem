using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.Models;

public partial class Chat
{
    public int chatId { get; set; }

    public string? Statuss { get; set; }

    public string? Channel { get; set; }

    public int? OrderID { get; set; }

    public virtual ICollection<CustomerChat> CustomerChats { get; set; } = new List<CustomerChat>();

    public virtual Order? Order { get; set; }

    public virtual ICollection<PharmacyChat> PharmacyChats { get; set; } = new List<PharmacyChat>();
}
