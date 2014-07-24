// -----------------------------------------------------------------------
// <copyright file="ShiftManagerViewModel.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

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
                    return Extensions.EnumToString((SupervisorActionType)SupervisorRole);
                }

                return string.Empty;
            }
        }
    }
}
