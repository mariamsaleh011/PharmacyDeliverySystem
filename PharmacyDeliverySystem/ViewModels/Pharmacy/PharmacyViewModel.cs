using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmacyDeliverySystem.ViewModels.Pharmacy
{
    public class PharmacyViewModels
    {
        public class PharmacyInputViewModel
        {
            public int? PharmId { get; set; }

            [Required(ErrorMessage = "Name مطلوب")]
            [StringLength(100, ErrorMessage = "Name لا يمكن أن يزيد عن 100 حرف")]
            public string Name { get; set; } = null!;
        }

        public class PharmacyResponseViewModel
        {
            public int PharmId { get; set; }

            public string Name { get; set; } = null!;

            public int ChatsCount { get; set; }
            public int OrdersCount { get; set; }
            public int PrescriptionsCount { get; set; }
            public int ProductsCount { get; set; }

            public List<int> OrderIds { get; set; } = new();
            public List<int> ProductIds { get; set; } = new();
            public List<int> ChatIds { get; set; } = new();
            public List<int> PrescriptionIds { get; set; } = new();
        }
    }
}
