// --------------------------------------------------------------------------
// <copyright file="SettingsSendMail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.2.13">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum SettingsSendMail : int
    {
        /// <summary>
        /// Create New Action.
        /// </summary>
        [StringValue("New")]
        New = 1,

        /// <summary>
        /// Verify Action.
        /// </summary>
        [StringValue("Verify")]
        Verify = 2,

        /// <summary>
        /// NotVerify Action.
        /// </summary>
        [StringValue("NotVerify")]
        NotVerify = 3,

        /// <summary>
        /// Approve Action.
        /// </summary>
        [StringValue("Approve")]
        Approve = 4,

        /// <summary>
        /// Reject Action.
        /// </summary>
        [StringValue("Reject")]
        Reject = 5,

        /// <summary>
        /// Process Action.
        /// </summary>
        [StringValue("Process")]
        Process = 6,

        /// <summary>
        /// Void Action.
        /// </summary>
        [StringValue("Void")]
        Void = 7,

        /// <summary>
        /// Reset1 Action.
        /// </summary>
        [StringValue("Reset1")]
        Reset1 = 8,

        /// <summary>
        /// Reset2 Action.
        /// </summary>
        [StringValue("Reset2")]
        Reset2 = 9,

        /// <summary>
        /// Reset3 Action.
        /// </summary>
        [StringValue("Reset3")]
        Reset3 = 10,

        /// <summary>
        /// Edit Action.
        /// </summary>
        [StringValue("Edit")]
        Edit = 11,

        /// <summary>
        /// Cancel Action.
        /// </summary>
        [StringValue("Cancel")]
        Cancel = 12,

        /// <summary>
        /// Reedit Action.
        /// </summary>
        [StringValue("Reedit")]
        Reedit = 13,

        /// <summary>
        /// Delete Action.
        /// </summary>
        [StringValue("Delete")]
        Delete = 14
    }
}
