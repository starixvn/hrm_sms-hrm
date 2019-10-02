namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Constants;

    public class LeaveRequest : BaseRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        public IList<LeaveRequestDetail> LeaveDetail { get; set; }

        public bool IsHalfDay => TotalDays == Constants.HalfDayOffValue;
    }
}