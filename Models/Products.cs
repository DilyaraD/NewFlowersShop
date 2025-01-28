namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Products
    {
        public Products()
        {
            //ProductContents = new HashSet<ProductContents>();
        }
    
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public string DescriptionProduct { get; set; }
        public string CareDescription { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Package { get; set; }
        public int StatusID { get; set; }
        public string Photo { get; set; }
        //public virtual ICollection<ProductContents> ProductContents { get; set; }
    }
}
