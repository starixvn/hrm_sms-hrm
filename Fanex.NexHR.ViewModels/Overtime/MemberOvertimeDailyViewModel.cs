namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class MemberOvertimeDailyViewModel
    {
        public DateTime Month { get; set; }

        public IList<OvertimeDaily> DailyOvertimes { get; set; }

        public RequestEntity MonthlyOvertime { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsHRView { get; set; }
    }
}