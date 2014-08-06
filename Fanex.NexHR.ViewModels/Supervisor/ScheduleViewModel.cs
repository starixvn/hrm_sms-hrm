// --------------------------------------------------------------------------
// <copyright file="ScheduleViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Supervisor
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Schedule ViewModel.
    /// </summary>
    public class ScheduleViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<ScheduleArrangeViewModel> ScheduleArranges { get; set; }
    }
}
