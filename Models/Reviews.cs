namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reviews
    {
        public int ReviewID { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public int StatusID { get; set; }
        public System.DateTime ReviewDate { get; set; }
    }
}
