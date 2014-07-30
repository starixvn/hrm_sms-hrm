// --------------------------------------------------------------------------
// <copyright file="CurrentRequestStatus.cs" company="Nexcel Solutions Vietnam">
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
    public enum CurrentRequestStatus : int
    {
        /// <summary>
        /// Pending item.
        /// </summary>
        [StringValue("Pending")]
        Pending = 1,

        /// <summary>
        /// Accepted item.
        /// </summary>
        [StringValue("Verified")]
        Verified = 2,

        /// <summary>
        /// NotAccepted item.
        /// </summary>
        [StringValue("NotVerified")]
        NotVerified = 3,

        /// <summary>
        /// Approved item.
        /// </summary>
        [StringValue("Approved")]
        Approved = 4,

        /// <summary>
        /// Rejected item.
        /// </summary>
        [StringValue("Rejected")]
        Rejected = 5,

        /// <summary>
        /// Processed item.
        /// </summary>
        [StringValue("Processed")]
        Processed = 6,

        /// <summary>
        /// Void item.
        /// </summary>
        [StringValue("Void")]
        Void = 7
    }
}
