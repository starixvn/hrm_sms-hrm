namespace Fanex.NexHR.Criteria.EmployeeRelationship
{
    using System;
    using Fanex.Data.Repository;

    public class EditRelationshipCriteria : CriteriaBase
    {
        public int RelationshipId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public int Modifier { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeRelationship_Edit";
        }

        public override bool IsValid()
        {
            return RelationshipId > 0 && Modifier > 0 && !string.IsNullOrEmpty(FullName);
        }
    }
}