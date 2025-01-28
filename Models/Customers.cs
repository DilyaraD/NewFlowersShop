namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customers
    {
        public Customers()
        {

        }
    
        public int CustomerID { get; set; }
        public string LoginCustomer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordCustomer { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
