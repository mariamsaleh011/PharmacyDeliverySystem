using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Chat
{
    public class ChatViewModels
    {
        // لإنشاء أو تعديل Chat
        public class ChatInputViewModel
        {
            [Required(ErrorMessage = "Status مطلوب")]
            [StringLength(50, ErrorMessage = "Status لا يمكن أن يزيد عن 50 حرف")]
            public string Status { get; set; } = string.Empty;   // ✅ قيمة افتراضية

            [StringLength(50, ErrorMessage = "Channel لا يمكن أن يزيد عن 50 حرف")]
            public string? Channel { get; set; }

            [Required(ErrorMessage = "OrderId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "OrderId لازم يكون أكبر من صفر")]
            public int OrderId { get; set; }

            [Required(ErrorMessage = "CustomerId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "CustomerId لازم يكون أكبر من صفر")]
            public int CustomerId { get; set; }

            [Required(ErrorMessage = "PharmacyId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "PharmacyId لازم يكون أكبر من صفر")]
            public int PharmacyId { get; set; }
        }

        // لإنشاء رسالة
        public class ChatMessageInputViewModel
        {
            [Required(ErrorMessage = "MessageText مطلوب")]
            [StringLength(1000, ErrorMessage = "الرسالة لا يمكن أن تزيد عن 1000 حرف")]
            public string MessageText { get; set; } = string.Empty;   // ✅ قيمة افتراضية

            public DateTime? SentAt { get; set; }
        }
    }
}
