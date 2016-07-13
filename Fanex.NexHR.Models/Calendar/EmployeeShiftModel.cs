namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeShiftModel
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public Shift Shift { get; set; }
    }
}