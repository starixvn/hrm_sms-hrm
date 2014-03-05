// --------------------------------------------------------------------------
// <copyright file="WorkingTimeRequest.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// WorkingTime Request.
    /// </summary>
    public class WorkingTimeRequest
    {
        public long RequestId { get; set; }

        public short RequestType { get; set; }

        public int ShiftId { get; set; }

        public string Shift { get; set; }

        public string ShiftName { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime OnDate { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? TotalDays { get; set; }

        public int? LeaveType { get; set; }

        public string LeaveCode { get; set; }

        public string LeaveName { get; set; }

        public string Remark { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte Status { get; set; }

        public int RequesterId { get; set; }

        public string Requester { get; set; }

        public string FullNameRequester { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public int VerifierId { get; set; }

        public int Time { get; set; }

        public int ReworkCount { get; set; }

        public string ToShiftName { get; set; }
    }
}
