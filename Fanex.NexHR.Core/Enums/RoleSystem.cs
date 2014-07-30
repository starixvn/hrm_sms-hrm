// --------------------------------------------------------------------------
// <copyright file="RoleSystem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum RoleSystem : int
    {
        /// <summary>
        /// Member item.
        /// </summary>
        [StringValue("Member")]
        Member = 1,

        /// <summary>
        /// AssistantLeader item.
        /// </summary>
        [StringValue("Verifier")]
        Verifier = 2,

        /// <summary>
        /// Leader item.
        /// </summary>
        [StringValue("Approver")]
        Approver = 3,

        /// <summary>
        /// ShiftManager item.
        /// </summary>
        [StringValue("ShiftManager")]
        ShiftManager = 4,

        /// <summary>
        /// ActingSupervisor item.
        /// </summary>
        [StringValue("ActingSupervisor")]
        ActingSupervisor = 5,

        /// <summary>
        /// Human Resource item.
        /// </summary>
        [StringValue("HR")]
        HR = 6,

        /// <summary>
        /// SystemAdmin item.
        /// </summary>
        [StringValue("SystemAdmin")]
        SystemAdmin = 7
    }
}