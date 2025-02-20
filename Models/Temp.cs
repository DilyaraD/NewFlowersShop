namespace NewFlowersShop.Models
{
    public class ReviewViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Photo { get; set; }
    }

    public class ReviewInputModel
    {
        public int ProductId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
    }

    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Photo { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusID { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public Deliveries Delivery { get; set; }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
    public class OrderDelivery2
    {
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryMethod { get; set; }

    }


    public class OrderDelivery3
    {
        public DateTime DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
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

    public class OrderViewModel3
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusID { get; set; }
        public int DeliveryID { get; set; }
        public OrderDelivery3 Delivery { get; set; }
        public List<Product2> Products { get; set; }
    }


}
