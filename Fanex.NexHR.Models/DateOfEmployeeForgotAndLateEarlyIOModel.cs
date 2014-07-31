// --------------------------------------------------------------------------
// <copyright file="DateOfEmployeeForgotAndLateEarlyIOModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.12.19">Created.</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// DateOfEmployeeForgotAndLateEarlyIO Model.
    /// </summary>
    public class DateOfEmployeeForgotAndLateEarlyIOModel
    {
        public DateTime AttendanceDate { get; set; }

        public bool IsLateIn { get; set; }

        public bool IsEarlyOut { get; set; }

        public bool IsMissingCheckIn { get; set; }

        public bool IsMissingCheckOut { get; set; }
    }
}
