// --------------------------------------------------------------------------
// <copyright file="SupervisorModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.10.22">Created</change>
// </history>

namespace Fanex.NexHR.Models.Supervisor
{
    /// <summary>
    /// Supervisor Model.
    /// </summary>
    public class SupervisorModel
    {
        public Employee Supervisor { get; set; }

        public int OwnerId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Team { get; set; }

        public byte ActionType { get; set; }

        public bool CheckInOuts { get; set; }

        public bool LateInEarlyOuts { get; set; }

        public bool ChangeShifts { get; set; }

        public bool Leaves { get; set; }

        public bool KeepInViews { get; set; }

        public bool Overtimes { get; set; }

        public bool ArrangeShifts { get; set; }

        public string OrderCode { get; set; }
    }
}
