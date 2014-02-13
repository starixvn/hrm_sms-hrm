// --------------------------------------------------------------------------
// <copyright file="WorkTypes.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.2.12">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum WorkTypes : short
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
        Work = 3,
    }
}
