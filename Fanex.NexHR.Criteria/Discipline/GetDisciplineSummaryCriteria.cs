namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        public int? ReasonId { get; set; }

        public int ApproverId { get; set; }

        public bool IsShowAll { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string GroupName { get; set; }

        public override bool IsValid() => FromDate <= ToDate && ApproverId > 0;

        public override string GetSettingKey() => "Report_DisciplineSummary";
    }
}