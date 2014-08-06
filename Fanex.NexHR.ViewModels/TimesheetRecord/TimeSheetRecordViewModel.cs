// --------------------------------------------------------------------------
// <copyright file="TimeSheetRecordViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.TimesheetRecord
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Time sheet record ViewModel.
    /// </summary>
    public class TimeSheetRecordViewModel
    {
        [Required]
        public int EmpId { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        public DateTime RecordTime { get; set; }

        public DateTime InsertDate { get; set; }

        [Required]
        public short InOutType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ForDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        public int Hour { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        public int Minute { get; set; }
    }
}
