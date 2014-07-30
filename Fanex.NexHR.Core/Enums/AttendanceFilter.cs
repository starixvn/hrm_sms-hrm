// --------------------------------------------------------------------------
// <copyright file="AttendanceFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.12.01">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum AttendanceFilter : int
    {
        /// <summary>
        /// Display all item.
        /// </summary>
        [StringValue("All")]
        All = 0,

        /// <summary>
        /// MissingCheckIn item.
        /// </summary>
        [StringValue("MissingCheckIn")]
        MissingCheckIn = 1,

        /// <summary>
        /// MissingCheckOut item.
        /// </summary>
        [StringValue("MissingCheckOut")]
        MissingCheckOut = 2,

        /// <summary>
        /// LateIn item.
        /// </summary>
        [StringValue("LateIn")]
        LateIn = 3,

        /// <summary>
        /// EarlyOut item.
        /// </summary>
        [StringValue("EarlyOut")]
        EarlyOut = 4,

        /// <summary>
        /// NotWork item.
        /// </summary>
        [StringValue("NotWork")]
        NotWork = 5
    }
}
