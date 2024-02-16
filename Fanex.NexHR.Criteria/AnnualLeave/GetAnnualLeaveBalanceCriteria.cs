namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class GetAnnualLeaveBalanceCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }
        public int? EmpId { get; set; }

        public string TeamIds { get; set; }

        public int Year { get; set; }

        public byte? IsLocal { get; set; }

        public int? SupervisorId { get; set; }

        public string Role { get; set; }

        public override string GetSettingKey() => "Report_LeaveSummary";

        public override bool IsValid() => true;
    }
}
