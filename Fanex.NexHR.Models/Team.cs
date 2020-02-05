namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;

    public class Team : EntityBase<int>
    {
        public Department Department { get; set; } = new Department();

        public IList<TeamLeader> Leaders { get; set; } = new List<TeamLeader>();

        public IList<TeamLeader> AssistantLeaders { get; set; } = new List<TeamLeader>();

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