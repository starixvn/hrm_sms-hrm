namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        public int ApproverId { get; set; }

        public override bool IsValid()
        {
            return FromDate <= ToDate && ApproverId > 0;
        }

        public override string GetSettingKey()
        {
            return "Discipline_GetSummary";
        }
    }
}