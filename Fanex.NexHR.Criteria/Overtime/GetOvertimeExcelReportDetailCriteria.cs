namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeExcelReportDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public string JobTitleId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Report_OvertimeDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}