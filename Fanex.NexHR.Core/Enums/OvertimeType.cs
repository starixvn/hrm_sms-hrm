// --------------------------------------------------------------------------
// <copyright file="OvertimeType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.25">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum OvertimeType : int
    {
        /// <summary>
        /// Normal item.
        /// </summary>
        [StringValue("Normal")]
        Normal = 1,

        /// <summary>
        /// Off Type item.
        /// </summary>
        [StringValue("Off")]
        Off = 2,

        /// <summary>
        /// Holiday item.
        /// </summary>
        [StringValue("Holiday")]
        Holiday = 3,
    }
}
