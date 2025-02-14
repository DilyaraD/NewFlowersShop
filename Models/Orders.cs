namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orders
    {
        public Orders()
        {

        }
    
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusID { get; set; }
        public int DeliveryID { get; set; }

    }
}
