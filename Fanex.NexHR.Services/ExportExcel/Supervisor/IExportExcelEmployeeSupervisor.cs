namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Supervisor;

    public interface IExportExcelEmployeeSupervisor
    {
        void Export(IList<SupervisorReportModel> supervisors);
    }
}