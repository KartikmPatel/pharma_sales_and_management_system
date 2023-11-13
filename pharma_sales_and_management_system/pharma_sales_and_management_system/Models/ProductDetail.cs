using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class ProductDetail
    {
        public ProductDetail()
        {
            AgencyOrders = new HashSet<AgencyOrder>();
            MedicalOrders = new HashSet<MedicalOrder>();
            MedicalSellingProducts = new HashSet<MedicalSellingProduct>();
            MedicalShopProductStocks = new HashSet<MedicalShopProductStock>();
            UserCarts = new HashSet<UserCart>();
            UserOrders = new HashSet<UserOrder>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public int RetailPrice { get; set; }
        public string ProductImage { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Disease { get; set; } = null!;
        public int CategoryId { get; set; }
        public DateTime MfgDate { get; set; }
        public int CompanyId { get; set; }
        public DateTime ExpDate { get; set; }

        public virtual ProductCategory Category { get; set; } = null!;
        public virtual Manufacturer Company { get; set; } = null!;
        public virtual ICollection<AgencyOrder> AgencyOrders { get; set; }
        public virtual ICollection<MedicalOrder> MedicalOrders { get; set; }
        public virtual ICollection<MedicalSellingProduct> MedicalSellingProducts { get; set; }
        public virtual ICollection<MedicalShopProductStock> MedicalShopProductStocks { get; set; }
        public virtual ICollection<UserCart> UserCarts { get; set; }
        public virtual ICollection<UserOrder> UserOrders { get; set; }
    }
}
