// --------------------------------------------------------------------------
// <copyright file="DeleteDepartmentCriteria.cs" company="Nexcel Solutions Vietnam">
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

    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Delete";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
