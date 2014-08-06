// --------------------------------------------------------------------------
// <copyright file="OvertimeRequest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// LateInEarlyOut Request.
    /// </summary>
    public class OvertimeRequest
    {
        public int Id { get; set; }

        public DateTime OvertimeDate { get; set; }

        public string Schedule { get; set; }

        public string TimeIn { get; set; }

        public string TimeOut { get; set; }

        public float TotalWorkHours { get; set; }

        public float NormalHours { get; set; }

        public float OffHours { get; set; }

        public float PublicHours { get; set; }

        public string Remark { get; set; }

        public int RequestId { get; set; }

        public int Status { get; set; }

        public int ShiftId { get; set; }

        public int EmpId { get; set; }

        public float TotalShiftHours { get; set; }

        public float WorkingHoursContract { get; set; }

        public string TeamName { get; set; }

        public string Team { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public bool IsOffDay { get; set; }

        public bool IsHoliday { get; set; }

        public bool IsKeepInView { get; set; }

        public bool IsChangeShift { get; set; }

        public string TotalWorkHoursTime { get; set; }
    }
}
