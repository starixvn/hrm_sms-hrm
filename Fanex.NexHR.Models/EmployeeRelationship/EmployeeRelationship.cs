namespace Fanex.NexHR.Models.EmployeeRelationship
{
    using System;

    public class EmployeeRelationship
    {
        public int RelationshipId { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public int Relationship { get; set; }
    }
}