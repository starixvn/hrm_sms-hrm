namespace GO.Website.Services
{
    using GO.Website.Models.Report.Manpower;

    public interface IExportExcelService
    {
        byte[] ExportManpowerReport(GridData groupByUnitData, GridData groupByCountryData);
    }
}