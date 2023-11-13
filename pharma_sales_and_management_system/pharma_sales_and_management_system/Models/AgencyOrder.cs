using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class AgencyOrder
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int AgencyId { get; set; }
        public int CompanyId { get; set; }
        public int IsDelivered { get; set; }

        public virtual AgencyDetail Agency { get; set; } = null!;
        public virtual Manufacturer Company { get; set; } = null!;
        public virtual ProductDetail Product { get; set; } = null!;
    }
}
