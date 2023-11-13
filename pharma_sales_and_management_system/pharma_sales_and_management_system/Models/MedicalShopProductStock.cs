using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class MedicalShopProductStock
    {
        public int Id { get; set; }
        public int MedicalShopId { get; set; }
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }

        public virtual MedicalShopDetail MedicalShop { get; set; } = null!;
        public virtual ProductDetail Product { get; set; } = null!;
    }
}
