// -----------------------------------------------------------------------
// <copyright file="ISupportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
