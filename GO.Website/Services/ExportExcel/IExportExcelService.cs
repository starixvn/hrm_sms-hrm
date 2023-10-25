namespace GO.Website.Services
{
    using System.Collections.Generic;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.Manpower;
    using EmployeeInfo = Models.Manpower.EmployeeInfo;

    public interface IExportExcelService
    {
        byte[] ExportAnnualManpowerForecastRequestForm(AnnualManpowerForecastRequest request);

        byte[] ExportJobLevelSummaryReport(GridData gridUnitData, GridData gridCountryData);

        byte[] ExportManpowerForecastReport(IEnumerable<AnnualManpowerForecastRequest> requests, int year, string periodName);
        byte[] ExportEmployeeReport(IEnumerable<EmployeeInfo> employeeInfos);
    }
}