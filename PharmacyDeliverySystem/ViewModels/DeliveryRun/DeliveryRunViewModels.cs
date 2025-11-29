using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.DeliveryRun
{
    public class DeliveryRunViewModels
    {
        public class CreateDeliveryRunViewModel
        {
            [Required(ErrorMessage = "RiderId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "RiderId لازم يكون أكبر من صفر")]
            public int RiderId { get; set; }

            [Required(ErrorMessage = "لازم تحدد أوردر واحد على الأقل")]
            [MinLength(1, ErrorMessage = "لازم يكون فيه أوردر واحد على الأقل")]
            public List<int> OrderIds { get; set; }
        }

        public class AddOrderToRunViewModel
        {
            [Required(ErrorMessage = "RunId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "RunId لازم يكون أكبر من صفر")]
            public int RunId { get; set; }

            [Required(ErrorMessage = "OrderId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "OrderId لازم يكون أكبر من صفر")]
            public int OrderId { get; set; }
        }

        public class CompleteRunViewModel
        {
            [Required(ErrorMessage = "RunId مطلوب")]
            [Range(1, int.MaxValue, ErrorMessage = "RunId لازم يكون أكبر من صفر")]
            public int RunId { get; set; }
        }
    }
}
