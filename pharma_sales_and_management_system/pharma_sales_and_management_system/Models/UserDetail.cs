using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            Feedbacks = new HashSet<Feedback>();
            UserCarts = new HashSet<UserCart>();
            UserOrders = new HashSet<UserOrder>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string City { get; set; } = null!;
        public int ContactNo { get; set; }
        public int Pincode { get; set; }
        public string? ProfilePic { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<UserCart> UserCarts { get; set; }
        public virtual ICollection<UserOrder> UserOrders { get; set; }
    }
}
