namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Supervisor;

    public interface IExportExcelSectionSupervisorService
    {
        void Export(IList<SectionSupervisorReportModel> supervisors);
    }
}