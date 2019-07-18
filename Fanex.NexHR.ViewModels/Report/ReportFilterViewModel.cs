namespace Fanex.NexHR.ViewModels
{
    using System;

    public class ReportFilterViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public int DateOption { get; set; }

        public bool IsExportExcel { get; set; }

        public byte? IsLocal { get; set; }
    }
}