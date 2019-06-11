namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class EditSectionSupervisorCriteria : CriteriaBase
    {
        public int GroupId { get; set; }
        public short RoleId { get; set; }
        public string SupervisorXml { get; set; }
        public int ActorId { get; set; }

        public override bool IsValid() => GroupId > 0 && ActorId > 0;

        public override string GetSettingKey() => "GroupSupervisor_Update";
    }
}