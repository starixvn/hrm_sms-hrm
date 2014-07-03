// --------------------------------------------------------------------------
// <copyright file="SupervisorModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.22">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Supervisor Model.
    /// </summary>
    public class SupervisorModel
    {
        public Employee Supervisor { get; set; }

        public bool HasLeaveRequest
        {
            get
            {
                return Leaves == 1 ? true : false;
            }
        }

        public bool HasChangeShiftRequest
        {
            get
            {
                return ChangeShifts == 1 ? true : false;
            }
        }

        public bool HasLateInEarlyOutRequest
        {
            get
            {
                return LateInEarlyOuts == 1 ? true : false;
            }
        }

        public bool HasCheckInOutRequest
        {
            get
            {
                return CheckInOuts == 1 ? true : false;
            }
        }

        public bool HasOvertimeRequest
        {
            get
            {
                return Overtimes == 1 ? true : false;
            }
        }

        public bool HasKeepInViewRequest
        {
            get
            {
                return KeepInViews == 1 ? true : false;
            }
        }

        public bool HasArrangeShiftRequest
        {
            get
            {
                return ArrangeShifts == 1 ? true : false;
            }
        }

        public int OwnerId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Team { get; set; }

        public byte ActionType { get; set; }

        public int CheckInOuts { get; set; }

        public int LateInEarlyOuts { get; set; }

        public int ChangeShifts { get; set; }

        public int Leaves { get; set; }

        public int KeepInViews { get; set; }

        public int Overtimes { get; set; }

        public int ArrangeShifts { get; set; }

        public string OrderCode { get; set; }
    }
}
