using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.QR
{
    public class QrConfirmationViewModels
    {
        // لإنشاء QR
        public class CreateQrRequest
        {
            [Required(ErrorMessage = "CustomerId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "CustomerId لازم يكون أكبر من صفر")]
            public int CustomerId { get; set; }

            [Required(ErrorMessage = "RunId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "RunId لازم يكون أكبر من صفر")]
            public int RunId { get; set; }
        }

        // لمسح QR
        public class ScanQrRequest
        {
            [Required(ErrorMessage = "ScannedBy مطلوب")]
            [StringLength(100, ErrorMessage = "ScannedBy لا يمكن أن يزيد عن 100 حرف")]
            public string ScannedBy { get; set; }
        }
    }
}
