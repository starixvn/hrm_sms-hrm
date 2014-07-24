// --------------------------------------------------------------------------
// <copyright file="TeamSupervisorReportModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Team Supervisor ReportModel.
    /// </summary>
    public class TeamSupervisorReportModel
    {
        public int GroupId { get; set; }

        public List<TeamSupervisorModel> SupervisorAction { get; set; }
    }
}
