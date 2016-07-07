namespace Fanex.NexHR.ViewModels.Supervisor
{
    using System;
    using System.Collections.Generic;

    public class ScheduleViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<ScheduleArrangeViewModel> ScheduleArranges { get; set; }
    }
}
