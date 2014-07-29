// -----------------------------------------------------------------------
// <copyright file="UpdateGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.29">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Group criteria.
    /// </summary>
    public class UpdateGroupCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string MailGroup { get; set; }

        public int LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Update";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}
