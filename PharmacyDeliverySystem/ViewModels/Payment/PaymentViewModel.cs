using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Payment
{
    public class PaymentViewModels
    {
        public class PaymentInputViewModel
        {
            [Required(ErrorMessage = "PayId مطلوب")]
            [StringLength(50, ErrorMessage = "PayId لا يمكن أن يزيد عن 50 حرف")]
            public string PayId { get; set; }

            [Required(ErrorMessage = "Status مطلوب")]
            [StringLength(50, ErrorMessage = "Status لا يمكن أن يزيد عن 50 حرف")]
            public string Status { get; set; }

            [Required(ErrorMessage = "Method مطلوب")]
            [StringLength(50, ErrorMessage = "Method لا يمكن أن يزيد عن 50 حرف")]
            public string Method { get; set; }
        }

        public class PaymentResponseViewModel
        {
            public string PayId { get; set; }
            public string Status { get; set; }
            public string Method { get; set; }

            public int OrdersCount { get; set; }
            public int RefundsCount { get; set; }

            public List<int> OrderIds { get; set; } = new();
            public List<int> RefundIds { get; set; } = new();
        }
    }
}
