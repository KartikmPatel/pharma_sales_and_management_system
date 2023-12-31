﻿using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class AgencyProductStock
    {
        public int Id { get; set; }
        public int TotalQuantity { get; set; }

        public int ProductId { get; set; }

        public virtual ProductDetail Product { get; set; } = null!;
    }
}
