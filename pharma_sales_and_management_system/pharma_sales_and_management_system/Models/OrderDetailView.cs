namespace pharma_sales_and_management_system.Models
{
    public class OrderDetailView
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string totalAmount { get; set; }
        public int is_delivered { get; set; }
        public string Address { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public int Mrp { get; set; }
        public string category { get; set; }
        public string Image { get; set; }
    }
}
