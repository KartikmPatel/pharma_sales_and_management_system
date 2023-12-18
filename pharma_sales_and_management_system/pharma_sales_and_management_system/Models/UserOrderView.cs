namespace pharma_sales_and_management_system.Models
{
    public class UserOrderView
    {
        public int Id { get; set; }
        public string productName { get; set; }
        public string Image { get; set; }
        public string userName { get; set; }
        public int quantity { get; set; }
        public int totalAmount { get; set; }
        public DateTime orderDate { get; set; }
        public int isDelivered { get; set; }
        public string orderAddress { get; set; }
    }
}
