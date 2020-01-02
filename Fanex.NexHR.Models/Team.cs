namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;

    public class Team : EntityBase<int>
    {
        public Team()
        {
            Department = new Department();
            Leaders = new List<Employee>();
            AssistantLeaders = new List<Employee>();
        }

        public Department Department { get; set; }

        public IList<Employee> Leaders { get; set; }

        public IList<Employee> AssistantLeaders { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        public int NoOfMembers { get; set; }

        public int TotalLocal { get; set; }

        public int TotalExpat { get; set; }

        public string MailGroup { get; set; }

        public byte? BasicAnnualLeaveBalance { get; set; }
    }
}