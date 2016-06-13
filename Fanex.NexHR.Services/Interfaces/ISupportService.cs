namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.ViewModels.TimesheetRecord;

    public interface ISupportService
    {
        bool RecordTimeSheet(TimeSheetRecordViewModel model);
    }
}
