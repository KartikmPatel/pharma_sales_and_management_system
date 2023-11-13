using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class MedicalShopDetail
    {
        public MedicalShopDetail()
        {
            Feedbacks = new HashSet<Feedback>();
            MedicalOrders = new HashSet<MedicalOrder>();
            MedicalSellingProducts = new HashSet<MedicalSellingProduct>();
            MedicalShopProductStocks = new HashSet<MedicalShopProductStock>();
            UserCarts = new HashSet<UserCart>();
            UserOrders = new HashSet<UserOrder>();
        }

        public int Id { get; set; }
        public string OwnerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ContactNo { get; set; }
        public string City { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ProfilePic { get; set; } = null!;

        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<MedicalOrder> MedicalOrders { get; set; }
        public virtual ICollection<MedicalSellingProduct> MedicalSellingProducts { get; set; }
        public virtual ICollection<MedicalShopProductStock> MedicalShopProductStocks { get; set; }
        public virtual ICollection<UserCart> UserCarts { get; set; }
        public virtual ICollection<UserOrder> UserOrders { get; set; }
    }
}
