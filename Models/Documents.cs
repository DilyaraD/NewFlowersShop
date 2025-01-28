namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class Documents
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public System.DateTime UploadDate { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> EmployeeID { get; set; }
    }
}
