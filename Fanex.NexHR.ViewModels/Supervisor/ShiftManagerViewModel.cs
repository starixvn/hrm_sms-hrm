// --------------------------------------------------------------------------
// <copyright file="ShiftManagerViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Supervisor
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Supervisor;

    /// <summary>
    /// Shift Manager ViewModel.
    /// </summary>
    public class ShiftManagerViewModel
    {
        public int EmployeeId { get; set; }

        public int TeamId { get; set; }

        public int SupervisorRole { get; set; }

        public string RoleShiftArrange { get; set; }

        public string ForView { get; set; }

        public IList<SupervisorModel> ShiftManagerList { get; set; }

        public string DisplayTitle
        {
            get
            {
                if (SupervisorRole > 0)
                {
                    return ((SupervisorActionType)SupervisorRole).ToString();
                }

                return string.Empty;
            }
        }
    }
}
