// --------------------------------------------------------------------------
// <copyright file="Team.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// Team Class.
    /// </summary>
    public class Team : EntityBase<int>
    {
        public Team()
        {
            Department = new Department();
            Leader = new Employee();
            AssistantLeader = new Employee();
        }

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
