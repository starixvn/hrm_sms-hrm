// --------------------------------------------------------------------------
// <copyright file="RequestFilter.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.3">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum JobTitleCategory
    {
        [StringValue("Default")]
        Default = 0,

        [StringValue("Worker")]
        Worker = 1,

        [StringValue("Staff")]
        Staff = 2,

        [StringValue("Executive")]
        Executive = 3,

        [StringValue("Executive With Position")]
        ExecutiveWithPosition = 4,

        [StringValue("Management")]
        Management = 5
    }
}
