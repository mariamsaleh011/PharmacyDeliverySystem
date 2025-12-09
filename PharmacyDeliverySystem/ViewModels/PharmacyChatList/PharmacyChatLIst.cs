using System;

namespace PharmacyDeliverySystem.ViewModels
{
    public class PharmacyChatListItemViewModel
    {
        public int ChatId { get; set; }
        public string CustomerName { get; set; } = "";
        public string LastMessage { get; set; } = "";
        public DateTime? LastMessageTime { get; set; }

        // هل الشات عليه رسائل جديدة (يستخدم في التقسيم New/My + البادجات)
        public bool IsNew { get; set; }

        // عدد الرسائل الجديدة في الشات ده
        public int UnreadCount { get; set; }
    }
}
