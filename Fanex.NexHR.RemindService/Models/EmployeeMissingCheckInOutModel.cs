// --------------------------------------------------------------------------
// <copyright file="EmployeeMissingCheckInOutModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Models
{
    using System;

    public class EmployeeMissingCheckInOutModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftCode { get; set; }

        public string Email { get; set; }

        public byte CheckInOutType { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }
    }
}
