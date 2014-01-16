// --------------------------------------------------------------------------
// <copyright file="ShiftStatisticContentViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.26">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    /// <summary>
    /// ShiftStatisticContent ViewModel.
    /// </summary>
    public class ShiftStatisticContentViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<ShiftStatisticModel> ShiftStatisticModels { get; set; }
    }
}
