namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Section;

    public class Team : EntityBase<int>
    {
        public Department Department { get; set; } = new Department();
        public IEnumerable<SectionManager> Managers { get; set; } = new List<SectionManager>();

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