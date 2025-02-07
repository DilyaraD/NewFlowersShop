namespace NewFlowersShop.Models
{
    public class OrderDelivModel
    {
            public int CustomerID { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string DeliveryMethod { get; set; }
            public string DeliveryAddress { get; set; }
            public string DeliveryTime { get; set; }
            public string PaymentMethod { get; set; }

    }
}
