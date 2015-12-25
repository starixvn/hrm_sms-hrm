// --------------------------------------------------------------------------
// <copyright file="GetEmployeeByShiftManagementCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.24">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Calendar
{
    using Fanex.Data.Repository;

    public class GetEmployeeByShiftManagementCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetEmployeeByShiftManagement";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
