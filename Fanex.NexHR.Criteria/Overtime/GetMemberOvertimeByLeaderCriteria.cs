namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetMemberOvertimeByLeaderCriteria : CriteriaBase
    {
        public int? SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestMonthlyByLeader";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
