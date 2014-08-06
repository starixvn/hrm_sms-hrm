// --------------------------------------------------------------------------
// <copyright file="SupportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    /// <summary>
    /// Support Service.
    /// </summary>
    public class SupportService : ISupportService
    {
        private readonly ISupportRepository _supportRepository;

        public SupportService(ISupportRepository supportRepository = null)
        {
            this._supportRepository = supportRepository ?? new SupportRepository();
        }

        public SupportService()
            : this(null)
        { 
        }

        public bool RecordTimeSheet(TimeSheetRecordViewModel model)
        {
            return _supportRepository.RecordTimeSheet(model);
        }
    }
}
