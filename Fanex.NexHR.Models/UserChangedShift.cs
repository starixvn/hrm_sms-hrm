// --------------------------------------------------------------------------
// <copyright file="UserChangedShift.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// User ChangedShift.
    /// </summary>
    public class UserChangedShift
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int Day { get; set; }

        public string Code { get; set; }

        public int ShiftId { get; set; }

        public string Code_From { get; set; }

        public int ShiftId_From { get; set; }
    }
}
