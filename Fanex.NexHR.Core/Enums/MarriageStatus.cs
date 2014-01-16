// --------------------------------------------------------------------------
// <copyright file="MarriageStatus.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.01.06">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum MarriageStatus
    {
        /// <summary>
        /// Default item.
        /// </summary>
        [StringValue("Default")]
        Default = 0,

        /// <summary>
        /// Married item.
        /// </summary>
        [StringValue("Married")]
        Married = 1,

        /// <summary>
        /// Divorced item.
        /// </summary>
        [StringValue("Divorced")]
        Divorced = 2,

        /// <summary>
        /// Single item.
        /// </summary>
        [StringValue("Single")]
        Single = 3,

        /// <summary>
        /// Widow item.
        /// </summary>
        [StringValue("Widow")]
        Widow = 4
    }
}
