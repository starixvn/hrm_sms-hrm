// -----------------------------------------------------------------------
// <copyright file="LocalLeave.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.08.13">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    /// <summary>
    /// The local leave.
    /// </summary>
    public enum LocalLeave
    {
        /// <summary>
        /// None item.
        /// </summary>
        [StringValue("None")]
        None = 0,

        /// <summary>
        /// Standard leave.
        /// </summary>
        [StringValue("Standard leave")]
        StandardLeave = 1,

        /// <summary>
        /// Accumulation leave.
        /// </summary>
        [StringValue("Per 5 Years accumulation leave")]
        AccumulationLeave = 2,

        /// <summary>
        /// Keep in view leave.
        /// </summary>
        [StringValue("Keep in view leave")]
        KIVLeave = 3
    }
}
