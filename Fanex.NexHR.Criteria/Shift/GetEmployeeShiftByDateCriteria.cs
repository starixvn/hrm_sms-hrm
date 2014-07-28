// --------------------------------------------------------------------------
// <copyright file="GetEmployeeShiftByDateCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.25">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Shift
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// GetEmployeeShiftByDate Criteria.
    /// </summary>
    public class GetEmployeeShiftByDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetShiftByDate";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
