// --------------------------------------------------------------------------
// <copyright file="ShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.6.30">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Shift ViewModel.
    /// </summary>
    public class ShiftViewModel
    {
        public int ShiftId { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MuniteTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeOut { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MuniteTimeOut { get; set; }

        [Required]
        public bool IsInAfterDay { get; set; }

        [Required]
        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsUsed { get; set; }
    }
}
