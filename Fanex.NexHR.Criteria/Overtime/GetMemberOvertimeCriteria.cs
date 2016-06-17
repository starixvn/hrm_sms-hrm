namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetMemberOvertimeCriteria : CriteriaBase
    {
        public int? SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestMonthlyByHR";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
