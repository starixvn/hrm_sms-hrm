// -----------------------------------------------------------------------
// <copyright file="ISupportRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISupportRepository
    {
        bool RecordTimeSheet(TimeSheetRecordViewModel model);
    }
}
