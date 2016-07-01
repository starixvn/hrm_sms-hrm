namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class UpdateSupervisorCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public short Role { get; set; }

        public string EmpIdLeaves { get; set; }

        public string EmpIdChangeShifts { get; set; }

        public string EmpIdLateInEarlyOuts { get; set; }

        public string EmpIdCheckInOuts { get; set; }

        public string EmpIdOvertimes { get; set; }

        public string EmpIdKeepInViews { get; set; }

        public string EmpIdDisciplines { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisorUpdate";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
