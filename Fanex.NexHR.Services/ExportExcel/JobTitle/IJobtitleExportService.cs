namespace Fanex.NexHR.Services.ExportExcel.JobTitle
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IJobTitleExportService
    {
        void ExportJobTitle(IEnumerable<LookupData> jobtitles);
    }
}