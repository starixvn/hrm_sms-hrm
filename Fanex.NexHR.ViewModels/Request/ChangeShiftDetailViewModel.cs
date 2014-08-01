// --------------------------------------------------------------------------
// <copyright file="ChangeShiftDetailViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.15">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Request
{
    using System;

    /// <summary>
    /// Shift Details ViewModel.
    /// </summary>
    public class ChangeShiftDetailViewModel
    {
        public DateTime OnDate { get; set; }

        public string ScheduleStatus { get; set; }

        public int FromShiftId { get; set; }

        public int ToShiftId { get; set; }

        public string Remark { get; set; }
    }
}
