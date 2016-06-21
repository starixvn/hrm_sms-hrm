// --------------------------------------------------------------------------
// <copyright file="SupervisorReportGroupModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.18">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    using System.Collections.Generic;

    /// <summary>
    /// Supervisor Report Group Model.
    /// </summary>
    public class SupervisorReportGroupModel
    {
        public int EmployeeId { get; set; }

        public List<SupervisorReportModel> SupervisorAction { get; set; }
    }
}
