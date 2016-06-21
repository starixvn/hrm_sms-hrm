// --------------------------------------------------------------------------
// <copyright file="ShiftStatisticViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.26">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    /// <summary>
    /// ShiftStatistic ViewModel.
    /// </summary>
    public class ShiftStatisticViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        public bool IsShowWholeCompany { get; set; }
    }
}
