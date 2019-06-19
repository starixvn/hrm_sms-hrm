namespace Fanex.NexHR.Criteria.EmployeeEmergency
{
    using System;
    using Fanex.Data.Repository;

    public class EditEmergencyCriteria : CriteriaBase
    {
        public int EmergencyId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public string Relationship { get; set; }

        public int Modifier { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeEmergency_Edit";
        }

        public override bool IsValid()
        {
            return EmergencyId > 0 && Modifier > 0 && !string.IsNullOrEmpty(FullName);
        }
    }
}