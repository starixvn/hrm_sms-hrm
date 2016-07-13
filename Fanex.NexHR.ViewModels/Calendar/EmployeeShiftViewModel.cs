namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeShiftViewModel
    {
        public DateTime WorkDate { get; set; }

        public int ShiftId { get; set; }

        public IList<Shift> Shifts { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public int? EmployeeId { get; set; }

        public byte? IsLocal { get; set; }

        public IList<Team> Teams { get; set; }
    }
}