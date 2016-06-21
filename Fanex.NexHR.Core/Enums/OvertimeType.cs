// --------------------------------------------------------------------------
// <copyright file="OvertimeType.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.25">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum OvertimeType : int
    {
        /// <summary>
        /// Normal item.
        /// </summary>
        [StringValue("Normal")]
        Normal = 1,

        /// <summary>
        /// Off Type item.
        /// </summary>
        [StringValue("Off")]
        Off = 2,

        /// <summary>
        /// Holiday item.
        /// </summary>
        [StringValue("Holiday")]
        Holiday = 3,
    }
}
