namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class MemberOvertimeViewModel
    {
        public DateTime Month { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int TeamId { get; set; }

        public int StatusId { get; set; }

        public bool IsHRView { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
