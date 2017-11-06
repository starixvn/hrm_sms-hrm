using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Models.Report
{
    public class WeekendLeaveDetail
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public bool IsLocal { get; set; }

        public int GroupId { get; set; }

        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public DateTime LeaveDate { get; set; }

        public float LeaveDay { get; set; }

        public string LeaveType { get; set; }

        public int LeaveTypeId { get; set; }
    }
}