using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public int UserId { get; set; }
        public int MedicalShopId { get; set; }

        public virtual MedicalShopDetail MedicalShop { get; set; } = null!;
        public virtual UserDetail User { get; set; } = null!;
    }
}
