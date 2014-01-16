// -----------------------------------------------------------------------
// <copyright file="ShiftManagerViewModel.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ShiftManagerViewModel
    {
        public int EmployeeId { get; set; }

        public int SupervisorRole { get; set; }

        public string RoleShiftArrange { get; set; }

        public IList<SupervisorModel> ShiftManagerList { get; set; }
    }
}
