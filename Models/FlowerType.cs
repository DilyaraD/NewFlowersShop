namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlowerType
    {
        public FlowerType()
        {
            
        }
    
        public int FlowerTypeID { get; set; }
        public int CategoryID { get; set; }
        public string FlowerTypeName { get; set; }
    }
}
