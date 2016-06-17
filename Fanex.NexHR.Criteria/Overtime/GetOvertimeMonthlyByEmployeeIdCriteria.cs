namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeMonthlyByEmployeeIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpID")]
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public string GroupIds { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetInOutMonthlyByEmpId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
