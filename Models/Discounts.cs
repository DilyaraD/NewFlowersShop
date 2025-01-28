namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Discounts
    {
        public Discounts()
        {

        }
    
        public int DiscountID { get; set; }
        public string DiscountName { get; set; }
        public decimal DiscountValue { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> EmployeeID { get; set; }
    }
}
