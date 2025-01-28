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
    }
}
