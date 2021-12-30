namespace Fanex.NexHR.ViewModels.EmployeeWorkFromHome
{
    using System;

    public class EmployeeWorkFromHomeFilterViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamIds { get; set; }
        public byte? IsLocal { get; set; }
    }
}