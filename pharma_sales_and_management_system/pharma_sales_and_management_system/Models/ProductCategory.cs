using System;
using System.Collections.Generic;

namespace pharma_sales_and_management_system.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
