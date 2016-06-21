// --------------------------------------------------------------------------
// <copyright file="SupervisorReportModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.17">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;

    public class SupervisorReportModel
    {
        public Employee Employee { get; set; }

        public string Supervisors { get; set; }

        public int ActionType { get; set; }

        public string ActionName
        {
            get
            {
                return Enumerations.GetStringValue((SupervisorActionType)ActionType);
            }
        }
    }
}
