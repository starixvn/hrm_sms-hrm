namespace Fanex.NexHR.Models
{
    using System;

    public class ShiftMultipleModel
    {
        public int EmployeeId { get; set; }

        public DateTime ForDate { get; set; }

        public int? ShiftId { get; set; }
    }
}