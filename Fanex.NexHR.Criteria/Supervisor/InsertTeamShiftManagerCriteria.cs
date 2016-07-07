namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class InsertTeamShiftManagerCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string ShiftArrangers { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_UpdateForShiftArrange";
        }
    }
}
