// --------------------------------------------------------------------------
// <copyright file="ApprovedStatus.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.20">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum ApprovedStatus
    {
        /// <summary>
        /// Pending item.
        /// </summary>
        [StringValue("Pending")]
        Pending = 1,

        /// <summary>
        /// Approved item.
        /// </summary>
        [StringValue("Approved")]
        Approved = 2,

        /// <summary>
        /// Rejected item.
        /// </summary>
        [StringValue("Rejected")]
        Rejected = 3
    }
}
