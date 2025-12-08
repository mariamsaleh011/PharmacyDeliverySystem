namespace PharmacyDeliverySystem.ViewModels
{
    public class PharmacyChatsPageViewModel
    {
        public List<PharmacyChatListItemViewModel> NewChats { get; set; }
            = new List<PharmacyChatListItemViewModel>();

        public List<PharmacyChatListItemViewModel> MyChats { get; set; }
            = new List<PharmacyChatListItemViewModel>();
    }
}
