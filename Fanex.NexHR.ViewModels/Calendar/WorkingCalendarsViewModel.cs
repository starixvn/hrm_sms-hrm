// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarsViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// WorkingCalendars ViewModel.
    /// </summary>
    public class WorkingCalendarsViewModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Team { get; set; }

        public int Days { get; set; }

        public string EmpCode { get; set; }

        public string Positions { get; set; }

        public List<string> WorkingDays { get; set; }

        public List<WorkingScheduleViewModel> WorkingSchedule { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
    }
}
