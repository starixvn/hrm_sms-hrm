// --------------------------------------------------------------------------
// <copyright file="RequestType.cs" company="Nexcel Solutions Vietnam">
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
    public enum RequestType : int
    {
        /// <summary>
        /// CheckIn item.
        /// </summary>
        [StringValue("Check In")]
        CheckIn = 1,

        /// <summary>
        /// CheckOut item.
        /// </summary>
        [StringValue("Check Out")]
        CheckOut = 2,

        /// <summary>
        /// LateIn item.
        /// </summary>
        [StringValue("Late In")]
        LateIn = 3,

        /// <summary>
        /// EarlyOut item.
        /// </summary>
        [StringValue("Early Out")]
        EarlyOut = 4,

        /// <summary>
        /// ChangeShift item.
        /// </summary>
        [StringValue("Change Shift")]
        ChangeShift = 5,

        /// <summary>
        /// Leave item.
        /// </summary>
        [StringValue("Leave")]
        Leave = 6,

        /// <summary>
        /// KeepInView item.
        /// </summary>
        [StringValue("KeepInView")]
        KeepInView = 7,

        /// <summary>
        /// Overtime item.
        /// </summary>
        [StringValue("Overtime")]
        Overtime = 8
    }
}
