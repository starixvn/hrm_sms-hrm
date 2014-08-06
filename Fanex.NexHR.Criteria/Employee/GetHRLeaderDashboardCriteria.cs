// --------------------------------------------------------------------------
// <copyright file="GetHRLeaderDashboardCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get HR Leader Dashboard Criteria.
    /// </summary>
    public class GetHRLeaderDashboardCriteria : CriteriaBase
    {
        public int Empid { get; set; }

        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "Report_HRLeaderDashBoard";
        }

        public override bool IsValid()
        {
            return Empid > 0;
        }
    }
}
