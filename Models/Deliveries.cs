namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Deliveries
    {
        public Deliveries()
        {

        }
    
        public int DeliveryID { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public int DeliveryStatusID { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryPayment { get; set; }
    }
}
