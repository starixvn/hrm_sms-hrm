// --------------------------------------------------------------------------
// <copyright file="RequestEntity.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Models.Request
{
    using System;

    /// <summary>
    /// Request Entity.
    /// </summary>
    public class RequestEntity : EntityBase<long>
    {
        public int EmpId { get; set; }

        public Employee Requester { get; set; }

        public DateTime RequestedDate { get; set; }

        public RequestType Type { get; set; }

        public RequestStatus Status { get; set; }

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee ModifiedBy { get; set; }

        public Employee VerifiedBy { get; set; }

        public Employee ApprovedBy { get; set; }

        public Employee HrBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public DateTime? HrDate { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public string RefNO { get; set; }

        public decimal TotalDaysRequestInMonth { get; set; }

        public int VerifiedStatus { get; set; }

        public int ApprovedStatus { get; set; }

        public int ProcessedStatus { get; set; }

        // Leave
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal TotalDays { get; set; }

        public int LeaveTypeId { get; set; }

        public string LeaveType { get; set; }

        public int ReworkCount { get; set; }

        // ChangeShift
        public Shift FromShift { get; set; }

        public Shift ToShift { get; set; }

        // LateIn-EarlyOut
        public int TotalMins { get; set; }

        // Overtime
        public short OvertimeType { get; set; }

        public bool IsOvertimeHours { get; set; }

        public int RequestType { get; set; }

        public decimal NormalHours { get; set; }

        public decimal OffHours { get; set; }

        public decimal PublicHours { get; set; }

        public decimal ToTalHours { get; set; }

        public bool IsShowWarning { get; set; }

        // Keep In View
        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string Shift { get; set; }

        public int ShiftId { get; set; }

        public bool IsHalfDay { get; set; }

        public bool CrossDay { get; set; }
    }
}
