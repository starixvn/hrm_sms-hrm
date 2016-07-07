namespace Fanex.NexHR.ViewModels.Request
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RequestsMonitoringViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Total Request >=")]
        public int MaxRequest { get; set; }

        public int DateOption { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }
    }
}
