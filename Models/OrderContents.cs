namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderContents
    {
        public int OrderContentID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
