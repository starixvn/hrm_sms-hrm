namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetYearlyOvertimeReportCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public int? ExcludedEmpID { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetRequestYearlyByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}