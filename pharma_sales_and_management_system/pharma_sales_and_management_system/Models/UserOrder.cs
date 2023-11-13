using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class UserOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MedicalShopId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int IsDelivered { get; set; }
        public string OrderAddress { get; set; } = null!;
        public string OrderCity { get; set; } = null!;

        public virtual MedicalShopDetail MedicalShop { get; set; } = null!;
        public virtual ProductDetail Product { get; set; } = null!;
        public virtual UserDetail User { get; set; } = null!;
    }
}
