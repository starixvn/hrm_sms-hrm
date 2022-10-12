namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class LogEmployeeLoginCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime LoginTime { get; set; }

        public string Platform { get; set; }

        public string IP { get; set; }

        public bool IsLogin { get; set; }

        public override string GetSettingKey() => "Employee_LogEmployeeLogin";

        public override bool IsValid() => EmployeeId > 0 && !string.IsNullOrEmpty(Platform) && !string.IsNullOrEmpty(IP);
    }
}