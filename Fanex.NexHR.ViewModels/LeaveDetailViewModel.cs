// --------------------------------------------------------------------------
// <copyright file="LeaveDetailViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.5">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// LeaveDetail ViewModel.
    /// </summary>
    public class LeaveDetailViewModel
    {
        public IList<LeaveDetailModel> LeaveRequestDetail { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }
    }
}
