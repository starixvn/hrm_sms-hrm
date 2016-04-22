namespace Fanex.NexHR.Criteria.Discipline
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        public int? ReasonId { get; set; }

        public int ApproverId { get; set; }

        public bool? HasIgnored { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return FromDate <= ToDate && ApproverId > 0;
        }

        public override string GetSettingKey()
        {
            return "Discipline_GetDetail";
        }
    }
}