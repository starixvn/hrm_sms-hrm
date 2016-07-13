namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyOvertimeReportByNormalCalendarCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ExcludedEmpID { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetNormalMonthlyByHR";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}