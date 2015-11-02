// --------------------------------------------------------------------------
// <copyright file="TeamSupervisorModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.23">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;

    public class TeamSupervisorModel
    {
        public int TeamId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int ActionType { get; set; }

        public string Supervisors { get; set; }

        public string ActionName
        {
            get
            {
                if (ActionType > 0)
                {
                    return Enumerations.GetStringValue((SupervisorActionType)ActionType);
                }

                return string.Empty;
            }
        }
    }
}
