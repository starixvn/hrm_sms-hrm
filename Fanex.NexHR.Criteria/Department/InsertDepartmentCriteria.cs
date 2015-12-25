// --------------------------------------------------------------------------
// <copyright file="InsertDepartmentCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Department
{
    using System;
    using Fanex.Data.Repository;

    public class InsertDepartmentCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int ParentId { get; set; }

        public int ManagerId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
