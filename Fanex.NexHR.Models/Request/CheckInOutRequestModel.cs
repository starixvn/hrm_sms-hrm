// --------------------------------------------------------------------------
// <copyright file="CheckInOutRequestModel.cs" company="Nexcel Solutions Vietnam">
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
    /// CheckInOut Request.
    /// </summary>
    public class CheckInOutRequestModel
    {
        public long RequestId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime OnDate { get; set; }

        public short RequestType { get; set; }

        public DateTime RequestTime { get; set; }

        public string RefNo { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
