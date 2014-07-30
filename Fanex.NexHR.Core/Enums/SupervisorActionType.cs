// --------------------------------------------------------------------------
// <copyright file="SupervisorActionType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.23">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum SupervisorActionType
    {
        /// <summary>
        /// Approved by.
        /// </summary>
        [StringValue("Verified by")]
        Verifier = 2,

        /// <summary>
        /// Approved by.
        /// </summary>
        [StringValue("Approved by")]
        Approver = 3,

        /// <summary>
        /// Shift Arrangement.
        /// </summary>
        [StringValue("Shift Arrangement")]
        ShiftArrangement = 4,

        /// <summary>
        /// On Behalf Request.
        /// </summary>
        [StringValue("On Behalf Request")]
        OnBehalfRequest = 5
    }
}
