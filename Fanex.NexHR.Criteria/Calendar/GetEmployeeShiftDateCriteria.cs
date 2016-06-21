// --------------------------------------------------------------------------
// <copyright file="GetEmployeeShiftDateCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.22">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeShiftDateCriteria : CriteriaBase
    {
        public int SupervisorID { get; set; }

        public int? ShiftId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string TeamIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Shift_GetByDate";
        }

        public override bool IsValid()
        {
            return SupervisorID > 0;
        }
    }
}
