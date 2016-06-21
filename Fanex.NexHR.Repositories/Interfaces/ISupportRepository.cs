// --------------------------------------------------------------------------
// <copyright file="ISupportRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// Interface Support Repository.
    /// </summary>
    public interface ISupportRepository
    {
        bool RecordTimeSheet(TimeSheetRecordViewModel model);
    }
}
