// -----------------------------------------------------------------------
// <copyright file="GetYearlyOvertimeReportByNormalCalendarCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Request.Overtime
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get yearly overtime by normal calendar criteria.
    /// </summary>
    public class GetYearlyOvertimeReportByNormalCalendarCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int Year { get; set; }

        public string Status { get; set; }

        public int ExcludedEmpID { get; set; }

        public string Role { get; set; }

        public bool IsDebug { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetNormalYearlyByHR";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
