namespace pharma_sales_and_management_system.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int totalAmount { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string productName { get; set; }
        public int is_delivered { get; set; }
    }
}
