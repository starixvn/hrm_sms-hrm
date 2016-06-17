namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeMonthlyByHRCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "EmpID")]
        public int EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetOTMonthlyOfEmpByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0 && EmployeeId > 0;
        }
    }
}
