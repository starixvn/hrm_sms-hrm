// --------------------------------------------------------------------------
// <copyright file="SupervisorViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    /// <summary>
    /// Supervisor ViewModel.
    /// </summary>
    public class SupervisorViewModel
    {
        public int EmployeeId { get; set; }

        public int TeamId { get; set; }

        public short SupervisorRole { get; set; }

        public IList<SupervisorModel> SupervisorList { get; set; }

        public string RoleLeaves { get; set; }

        public string RoleChangeShifts { get; set; }

        public string RoleLateInEarlyOuts { get; set; }

        public string RoleCheckInOuts { get; set; }

        public string RoleOvertimes { get; set; }

        public string RoleKeepInViews { get; set; }

        public string ForView { get; set; }

        public string DisplayTitle
        {
            get
            {
                if (SupervisorRole > 0)
                {
                    return Extensions.EnumToString((SupervisorActionType)SupervisorRole);
                }

                return string.Empty;
            }
        }
    }
}
