// --------------------------------------------------------------------------
// <copyright file="OvertimeExcelReportViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Overtime
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Overtime;

    /// <summary>
    /// Overtime Excel Report ViewModel.
    /// </summary>
    public class OvertimeExcelReportViewModel
    {
        public string Name { get; set; }

        public List<OvertimeExcelReport> ListOvertimeEntity { get; set; }
    }
}
