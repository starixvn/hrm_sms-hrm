// --------------------------------------------------------------------------
// <copyright file="InsertShiftManagerCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.EmployeeSupervisor
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Insert Shift Manager Criteria.
    /// </summary>
    public class InsertShiftManagerCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public string EmpIdShift { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_InsertEmpMngShiftList";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
