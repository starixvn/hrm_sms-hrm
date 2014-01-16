// --------------------------------------------------------------------------
// <copyright file="ShiftDetailsViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.15">Created</change>
// </history>

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
    public class ShiftDetailsViewModel
    {
        public string Date { get; set; }

        public string ScheduleStatus { get; set; }

        public int FromShift { get; set; }

        public int ToShift { get; set; }

        public IList<Shift> ChangeToShift { get; set; }

        public string Remark { get; set; }

        public string ChangeToShiftCode { get; set; }

        public int ShiftId { get; set; }

        public bool HasData { get; set; }
    }
}
