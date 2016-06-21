// --------------------------------------------------------------------------
// <copyright file="VerifiedStatus.cs" company=".">
// Copyright (c) .. All rights reserved.
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
    public enum VerifiedStatus
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
        NotVerified = 3
    }
}
