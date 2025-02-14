namespace NewFlowersShop.Models
{
    public class OrderDelivery2
    {
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryMethod { get; set; }

    }

    public class Product2
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
    }

    public class OrderViewModel2
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusID { get; set; }
        public int DeliveryID { get; set; }
    public OrderDelivery2 Delivery { get; set; }
        public List<Product2> Products { get; set; }
    }

}
