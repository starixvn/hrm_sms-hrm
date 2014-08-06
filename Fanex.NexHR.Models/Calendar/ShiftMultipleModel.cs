// --------------------------------------------------------------------------
// <copyright file="ShiftMultipleModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.9">Created</change>
// </history>

namespace Fanex.NexHR.Models.Calendar
{
    using System;

    /// <summary>
    /// ShiftMultiple Model.
    /// </summary>
    public class ShiftMultipleModel
    {
        public int EmployeeId { get; set; }

        public DateTime ForDate { get; set; }

        public int ShiftId { get; set; }
    }
}
