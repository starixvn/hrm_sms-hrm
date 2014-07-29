// -----------------------------------------------------------------------
// <copyright file="DeleteGroupCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Delete Group criteria.
    /// </summary>
    public class DeleteGroupCriteria : CriteriaBase
    {
        public int GroupID { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Delete";
        }

        public override bool IsValid()
        {
            return GroupID > 0;
        }
    }
}
