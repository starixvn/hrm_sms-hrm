namespace Fanex.NexHR.Models.Request
{
    using System;

    public class ChangeShiftDetailModel
    {
        public DateTime OnDate { get; set; }

        public int FromShiftId { get; set; }

        public string FromShift { get; set; }

        public int ToShiftId { get; set; }

        public string ToShift { get; set; }

        public string Remark { get; set; }
    }
}
