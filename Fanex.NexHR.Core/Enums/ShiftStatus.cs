// --------------------------------------------------------------------------
// <copyright file="ShiftStatus.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.1">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum ShiftStatus : int
    {
        /// <summary>
        /// The all item (active and disabled).
        /// </summary>
        [StringValue("All")]
        All = 0,

        /// <summary>
        /// Active item.
        /// </summary>
        [StringValue("Active")]
        Active = 1,

        /// <summary>
        /// Disabled item.
        /// </summary>
        [StringValue("Disabled")]
        Disabled = 2
    }
}
