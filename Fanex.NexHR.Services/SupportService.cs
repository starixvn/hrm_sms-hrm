// -----------------------------------------------------------------------
// <copyright file="SupportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
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
