namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ShiftStatisticViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        public bool IsShowWholeCompany { get; set; }
    }
}
