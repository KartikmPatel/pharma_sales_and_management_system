using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            AgencyOrders = new HashSet<AgencyOrder>();
            MedicalOrders = new HashSet<MedicalOrder>();
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int Id { get; set; }
        public string ComponyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ContactNo { get; set; }
        public string Password { get; set; } = null!;
        public string City { get; set; } = null!;

        public virtual ICollection<AgencyOrder> AgencyOrders { get; set; }
        public virtual ICollection<MedicalOrder> MedicalOrders { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
