// -----------------------------------------------------------------------
// <copyright file="MessageCategory.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.17">Created.</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    /// <summary>
    /// The category of message.
    /// </summary>
    public enum MessageCategory
    {
        /// <summary>
        /// Pending item.
        /// </summary>
        [StringValue("Notification")]
        Notification = 0,

        /// <summary>
        /// Processed item.
        /// </summary>
        [StringValue("Warning")]
        Warning = 1,

        /// <summary>
        /// Void item.
        /// </summary>
        [StringValue("Violation")]
        Violation = 2
    }
}
