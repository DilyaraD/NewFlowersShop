namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employees
    {
        public Employees()
        {
        }
    
        public int EmployeeID { get; set; }
        public string LoginEmployee { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordEmployee { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleID { get; set; }
    }
}
