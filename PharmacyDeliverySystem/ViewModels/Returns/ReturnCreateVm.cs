using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Returns
{
    public class ReturnCreateVm
    {
        [Required, Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Required, StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        // مفيش Required ولا Regex هنا
        // ونخلي القيمة الافتراضية متوافقة مع النظام بتاعك
        public string Status { get; set; } = "Pending";
    }
}
