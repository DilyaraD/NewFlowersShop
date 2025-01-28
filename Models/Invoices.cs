namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoices
    {
        public int InvoiceID { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int EmployeeID { get; set; }
    }
}
