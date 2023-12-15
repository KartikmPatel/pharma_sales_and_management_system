namespace pharma_sales_and_management_system.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Mrp { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }

        public int productId { get; set; }
        public int medicalId { get; set; }
    }
}
