// --------------------------------------------------------------------------
// <copyright file="ShiftStatisticContentViewModel.cs" company=".">
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
    using Fanex.NexHR.Models.Calendar;

    /// <summary>
    /// ShiftStatisticContent ViewModel.
    /// </summary>
    public class ShiftStatisticContentViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public IList<ShiftStatisticModel> ShiftStatisticModels { get; set; }
    }
}
