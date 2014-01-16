// -----------------------------------------------------------------------
// <copyright file="ISupportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISupportService
    {
        bool RecordTimeSheet(TimeSheetRecordViewModel model);
    }
}
