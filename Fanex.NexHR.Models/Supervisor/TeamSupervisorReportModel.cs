// --------------------------------------------------------------------------
// <copyright file="TeamSupervisorReportModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.24">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    using System.Collections.Generic;

    /// <summary>
    /// Team Supervisor ReportModel.
    /// </summary>
    public class TeamSupervisorReportModel
    {
        public int GroupId { get; set; }

        public IList<TeamSupervisorModel> SupervisorAction { get; set; }
    }
}
