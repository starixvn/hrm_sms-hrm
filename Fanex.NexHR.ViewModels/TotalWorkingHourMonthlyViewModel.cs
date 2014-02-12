// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourMonthlyViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.1.7">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TotalWorkingHourMonthlyViewModel ViewModel.
    /// </summary>
    public class TotalWorkingHourMonthlyViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime Month { get; set; }

        public string Team { get; set; }

        public string TeamIds { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        public bool IsExportExcel { get; set; }

        public byte? IsLocal { get; set; }
    }
}
