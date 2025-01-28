namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductContents
    {
        public int ProductContentID { get; set; }
        public int ProductID { get; set; }
        public int FlowerTypeID { get; set; }
        public int Quantity { get; set; }
        public virtual FlowerType FlowerType { get; set; }
    }
}
