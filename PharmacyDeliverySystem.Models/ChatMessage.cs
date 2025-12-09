using System;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        public int ChatId { get; set; }

        // ✅ قيم افتراضية عشان مايبقوش null
        public string SenderType { get; set; } = string.Empty;

        public string MessageText { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public virtual Chat? Chat { get; set; }  // Navigation للـ Chat
        public bool IsRead { get; set; } = false;

    }
}
