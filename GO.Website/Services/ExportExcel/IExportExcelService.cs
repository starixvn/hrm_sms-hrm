namespace GO.Website.Services
{
    using System.Collections.Generic;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.Manpower;

    public interface IExportExcelService
    {
        byte[] ExportManpowerReport(GridData gridUnitData, GridData gridCountryData);
        byte[] ExportManpowerForecastReport(IEnumerable<AnnualManpowerForecastRequest> request, int year);
    }
}