// --------------------------------------------------------------------------
// <copyright file="MemberOvertimeDailyViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.11">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// MemberOvertimeDaily ViewModel.
    /// </summary>
    public class MemberOvertimeDailyViewModel
    {
        public DateTime Month { get; set; }

        public IList<OvertimeRequest> DailyOvertimes { get; set; }

        public RequestEntity MonthlyOvertime { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsHRView { get; set; }
    }
}
