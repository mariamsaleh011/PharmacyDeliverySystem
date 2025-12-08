namespace PharmacyDeliverySystem.ViewModels
{
    public class PharmacyChatListItemViewModel
    {
        public int ChatId { get; set; }
        public string CustomerName { get; set; } = "";
        public string LastMessage { get; set; } = "";
        public DateTime? LastMessageTime { get; set; }
        public bool IsNew { get; set; }   // ← إضافة مهمة
    }
}
