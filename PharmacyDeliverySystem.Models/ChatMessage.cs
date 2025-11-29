using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PharmacyDeliverySystem.Models;

public class ChatMessage
{
    [Key]
    public int MessageId { get; set; }
    public int ChatId { get; set; }
    public string SenderType { get; set; }
    public string MessageText { get; set; }
    public DateTime SentAt { get; set; }

    public virtual Chat? Chat { get; set; }  // Navigation للـ Chat
}

