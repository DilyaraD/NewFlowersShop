namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class StoreFlowerStocks
    {
        public int StoreFlowerStockID { get; set; }
        public int StoreID { get; set; }
        public int FlowerTypeID { get; set; }
        public int Quantity { get; set; }
    }
}
