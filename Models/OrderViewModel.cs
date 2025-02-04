using static NewFlowersShop.Controllers.HomeController;

namespace NewFlowersShop.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusID { get; set; }
        public List<ProductViewModel> Products { get; set; }

            public Deliveries Delivery { get; set; } 
    }
}
