namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class WorkingCalendarsViewModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Team { get; set; }

        public int Days { get; set; }

        public string EmpCode { get; set; }

        public string Positions { get; set; }

        public List<string> WorkingDays { get; set; }

        public List<WorkingScheduleViewModel> WorkingSchedule { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
    }
}
