namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class InsertOrUpdateTeamSupervisorCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public int Role { get; set; }

        public string EmpIdLeaves { get; set; }

        public string EmpIdChangeShifts { get; set; }

        public string EmpIdLateInEarlyOuts { get; set; }

        public string EmpIdCheckInOuts { get; set; }

        public string EmpIdOvertimes { get; set; }

        public string EmpIdKeepInViews { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_Set";
        }
    }
}
