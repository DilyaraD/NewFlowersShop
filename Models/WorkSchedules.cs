namespace NewFlowersShop
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkSchedules
    {
        public int ScheduleID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime WorkDate { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
    }
}
