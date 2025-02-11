namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class RestockRequests
    {
        public int RestockRequestID { get; set; }
        public int ProductID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public int Quantity { get; set; }
        public int StatusID { get; set; }
        public int EmployeeID { get; set; }
        public int StoreID { get; set; }
    }
}
