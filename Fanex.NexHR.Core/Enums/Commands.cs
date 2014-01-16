// --------------------------------------------------------------------------
// <copyright file="Commands.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum Commands
    {
        /// <summary>
        /// Accept item.
        /// </summary>
        [StringValue("Accept")]
        Accept = 1,

        /// <summary>
        /// NotAccept item.
        /// </summary>
        [StringValue("NotAccept")]
        NotAccept = 2,

        /// <summary>
        /// Approve item.
        /// </summary>
        [StringValue("Approve")]
        Approve = 3,

        /// <summary>
        /// Reject item.
        /// </summary>
        [StringValue("Reject")]
        Reject = 4,

        /// <summary>
        /// Process item.
        /// </summary>
        [StringValue("Process")]
        Process = 5,

        /// <summary>
        /// Void item.
        /// </summary>
        [StringValue("Void")]
        Void = 6,

        /// <summary>
        /// Reset item.
        /// </summary>
        [StringValue("Reset")]
        Reset = 7
    }
}
