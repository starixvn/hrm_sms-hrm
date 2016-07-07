namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TotalWorkingHoursViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int DateOption { get; set; }

        public bool IsExportExcel { get; set; }

        public byte? IsLocal { get; set; }
    }
}
