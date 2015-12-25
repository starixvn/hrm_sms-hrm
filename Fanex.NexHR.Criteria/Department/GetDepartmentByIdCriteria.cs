// --------------------------------------------------------------------------
// <copyright file="GetDepartmentByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey()
        {
            return "Department_GetById";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
