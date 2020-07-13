namespace Fanex.NexHR.Models
{
    using Fanex.NexHR.Models.Group;
    using System;
    using System.Collections.Generic;

    public class Team : EntityBase<int>
    {
        public Section Section { get; set; } = new Section();

        public IEnumerable<TeamManager> Managers { get; set; } = new List<TeamManager>();

        public DateTime? CreatedDate { get; set; }

        public Employee Creator { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Employee Modifier { get; set; }

        public int NoOfMembers { get; set; }

        public int TotalLocal { get; set; }

        public int TotalExpat { get; set; }

        public string MailGroup { get; set; }

        public byte? BasicAnnualLeaveBalance { get; set; }

        public Department Department { get; set; }
    }
}