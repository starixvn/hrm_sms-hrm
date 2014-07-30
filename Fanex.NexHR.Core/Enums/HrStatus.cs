// --------------------------------------------------------------------------
// <copyright file="HrStatus.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.20">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum HRStatus
    {
        /// <summary>
        /// Pending item.
        /// </summary>
        [StringValue("Pending")]
        Pending = 1,

        /// <summary>
        /// Processed item.
        /// </summary>
        [StringValue("Processed")]
        Processed = 2,

        /// <summary>
        /// Void item.
        /// </summary>
        [StringValue("Void")]
        Void = 3
    }
}
