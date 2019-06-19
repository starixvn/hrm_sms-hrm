namespace Fanex.NexHR.Criteria.EmployeeEmergency
{
    using System;
    using Fanex.Data.Repository;

    public class InsertEmergencyCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public string Relationship { get; set; }

        public int Creator { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeEmergency_Insert";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && Creator > 0 && !string.IsNullOrEmpty(FullName);
        }
    }
}