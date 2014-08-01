// --------------------------------------------------------------------------
// <copyright file="GetRequestMonitoringCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Request Monitoring Criteria.
    /// </summary>
    public class GetRequestMonitoringCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Role { get; set; }

        public int LoginUserId { get; set; }

        public int? EmployeeId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public int MaxRequest { get; set; }

        public override string GetSettingKey()
        {
            return "Report_RequestMonitor";
        }

        public override bool IsValid()
        {
            return LoginUserId > 0;
        }
    }
}
