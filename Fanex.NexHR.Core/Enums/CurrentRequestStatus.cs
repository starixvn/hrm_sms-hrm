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
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Verified")]
        Verified = 2,

        [StringValue("NotVerified")]
        NotVerified = 3,

        [StringValue("Approved")]
        Approved = 4,

        [StringValue("Rejected")]
        Rejected = 5,

        [StringValue("Processed")]
        Processed = 6,

        [StringValue("Void")]
        Void = 7
    }
}
