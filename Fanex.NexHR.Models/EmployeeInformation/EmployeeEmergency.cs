namespace Fanex.NexHR.Models.EmployeeInformation
{
    using System;

    public class EmployeeEmergency
    {
        public int EmergencyId { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Relationship { get; set; }

        public int Gender { get; set; }
    }
}