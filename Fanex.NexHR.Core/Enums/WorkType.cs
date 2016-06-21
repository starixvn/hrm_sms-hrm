// --------------------------------------------------------------------------
// <copyright file="WorkType.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.2.12">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum WorkType : int
    {
        /// <summary>
        /// Off day item.
        /// </summary>
        [StringValue("Off")]
        Off = 1,

        /// <summary>
        /// Leave item.
        /// </summary>
        [StringValue("Leave")]
        Leave = 2,

        /// <summary>
        /// Work item.
        /// </summary>
        [StringValue("Work")]
        Work = 3
    }
}
