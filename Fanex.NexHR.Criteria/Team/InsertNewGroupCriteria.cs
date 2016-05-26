// --------------------------------------------------------------------------
// <copyright file="InsertNewGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert new group criteria.
    /// </summary>
    public class InsertNewGroupCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string MailGroup { get; set; }

        public int LeaderId { get; set; }

        public int? AssistantLeaderId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}