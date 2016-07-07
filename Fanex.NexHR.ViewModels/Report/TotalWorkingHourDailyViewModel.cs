namespace Fanex.NexHR.ViewModels.Report
{
    using System;

    public class TotalWorkingHourDailyViewModel
    {
        public DateTime WorkDate { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public bool IsExportExcel { get; set; }

        public byte? IsLocal { get; set; }
    }
}
