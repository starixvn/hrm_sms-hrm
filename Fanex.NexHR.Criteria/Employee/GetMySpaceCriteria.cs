// --------------------------------------------------------------------------
// <copyright file="GetMySpaceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get My Space Criteria.
    /// </summary>
    public class GetMySpaceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime? Date { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeDashBoard";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
