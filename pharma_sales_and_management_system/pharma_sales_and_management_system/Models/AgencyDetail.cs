using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class AgencyDetail
    {
        public AgencyDetail()
        {
            AgencyOrders = new HashSet<AgencyOrder>();
        }

        public int Id { get; set; }
        public string AgencyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ContactNo { get; set; }
        public string Password { get; set; } = null!;
        public string ProfileImage { get; set; } = null!;

        public virtual ICollection<AgencyOrder> AgencyOrders { get; set; }
    }
}
