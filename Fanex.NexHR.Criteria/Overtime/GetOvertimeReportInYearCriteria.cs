namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetOvertimeReportInYearCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public int Year { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestMonthlyByEmpId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}