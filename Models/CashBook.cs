namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class CashBook
    {
        public int CashBookID { get; set; }
        public System.DateTime OperationDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string OperationType { get; set; }
        public Nullable<int> EmployeeID { get; set; }
    }
}
