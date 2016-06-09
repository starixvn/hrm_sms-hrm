namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    public class SupportService : ISupportService
    {
        private readonly ISupportRepository supportRepository;

        public SupportService(ISupportRepository supportRepository = null)
        {
            this.supportRepository = supportRepository ?? new SupportRepository();
        }

        public SupportService()
            : this(null)
        { 
        }

        public bool RecordTimeSheet(TimeSheetRecordViewModel model)
        {
            return supportRepository.RecordTimeSheet(model);
        }
    }
}
