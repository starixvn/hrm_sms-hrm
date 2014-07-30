// --------------------------------------------------------------------------
// <copyright file="VerifiedStatus.cs" company="Nexcel Solutions Vietnam">
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
        [StringValue("Accepted")]
        Accepted = 2,

        /// <summary>
        /// NotAccepted item.
        /// </summary>
        [StringValue("NotAccepted")]
        NotAccepted = 3
    }
}
