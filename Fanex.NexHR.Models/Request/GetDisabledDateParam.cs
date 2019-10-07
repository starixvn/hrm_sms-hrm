namespace Fanex.NexHR.Models.Request
{
    using System;

    public class GetDisabledDateParam
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? RequestId { get; set; }
    }
}