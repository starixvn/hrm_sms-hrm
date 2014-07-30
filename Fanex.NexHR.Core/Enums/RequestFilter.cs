// --------------------------------------------------------------------------
// <copyright file="RequestFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.24">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RequestFilter : int
    {
        /// <summary>
        /// All value item.
        /// </summary>
        [StringValue("All")]
        All = 0,

        /// <summary>
        /// CheckInOut item.
        /// </summary>
        [StringValue("Check In/Check Out")]
        CheckInOut = 1,

        /// <summary>
        /// LateInEarlyOut item.
        /// </summary>
        [StringValue("Late In/Early Out")]
        LateInEarlyOut = 2,

        /// <summary>
        /// ChangeShift item.
        /// </summary>
        [StringValue("Change Shift")]
        ChangeShift = 3,

        /// <summary>
        /// Leave item.
        /// </summary>
        [StringValue("Leave")]
        Leave = 4,

        /// <summary>
        /// KeepInView item.
        /// </summary>
        [StringValue("KeepInView")]
        KeepInView = 5
    }
}
