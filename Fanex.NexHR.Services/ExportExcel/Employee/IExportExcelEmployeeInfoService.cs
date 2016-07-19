namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IExportExcelEmployeeInfoService
    {
        void ExportEmployeeBasicInfo(IList<Employee> employees);

        void ExportEmployeeDetailsInfo(IList<Employee> employees);
    }
}