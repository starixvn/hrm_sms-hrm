namespace Fanex.NexHR.Models.EmployeeInformation
{
    using System.Collections.Generic;

    public class EmployeeFullProfile
    {
        public Employee Employee { get; set; }

        public EmployeeEmergency Emergency { get; set; }

        public IEnumerable<EmployeeRelationship> Relationships { get; set; }
    }
}