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
    /// <summary>
    /// Supervisor Action Type.
    /// </summary>
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
