namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    public class GetYearlyOvertimeReportByNormalCalendarCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetNormalYearlyByHR";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
