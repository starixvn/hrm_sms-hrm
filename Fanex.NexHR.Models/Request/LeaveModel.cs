namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LeaveModel : BaseRequest
    {
        private const decimal HalfDayEnum = (decimal)0.5;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        public IList<LeaveDetailModel> LeaveDetail { get; set; }

        public bool IsHalfDay
        {
            get
            {
                return TotalDays == HalfDayEnum;
            }
        }
    }
}
