// -----------------------------------------------------------------------
// <copyright file="TimeSheetRecordViewModel.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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

        public bool InOutType { get; set; }

        public DateTime InsertDate { get; set; }

        [Required]
        public short Type { get; set; }

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
