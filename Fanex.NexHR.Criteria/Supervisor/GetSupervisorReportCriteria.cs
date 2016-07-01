namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetSupervisorReportCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public int? ActionType { get; set; }

        public byte? IsLocal { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_Get";
        }
    }
}
