namespace Fanex.NexHR.Models.Group
{
    using System;

    public class TeamModel
    {
        public TeamModel()
        {
            Department = new Department();
            Leader = new Employee();
            AssistantLeader = new Employee();
        }

        public int TeamId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public Department Department { get; set; }

        public Employee Leader { get; set; }

        public Employee AssistantLeader { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        public int NoOfMembers { get; set; }

        public int TotalLocal { get; set; }

        public int TotalExpat { get; set; }

        public string MailGroup { get; set; }
    }
}