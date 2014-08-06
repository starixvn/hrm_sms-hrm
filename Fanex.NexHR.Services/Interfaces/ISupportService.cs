// --------------------------------------------------------------------------
// <copyright file="ISupportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// Interface Support Service.
    /// </summary>
    public interface ISupportService
    {
        bool RecordTimeSheet(TimeSheetRecordViewModel model);
    }
}
