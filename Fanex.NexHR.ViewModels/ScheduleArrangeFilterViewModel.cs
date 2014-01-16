// --------------------------------------------------------------------------
// <copyright file="ScheduleArrangeFilterViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.19">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;

    /// <summary>
    /// ScheduleArrange ViewModel.
    /// </summary>
    public class ScheduleArrangeFilterViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Groups { get; set; }

        public IList<LookupData> JobTitles { get; set; }
    }
}
