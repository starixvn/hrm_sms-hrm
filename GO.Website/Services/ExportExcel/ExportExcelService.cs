namespace GO.Website.Services
{
    using System.IO;
    using GO.Website.Models.Report.Manpower;
    using Microsoft.AspNetCore.Hosting;

    public class ExportExcelService : IExportExcelService
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public ExportExcelService(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public byte[] ExportManpowerReport(GridData gridUnitData, GridData gridCountryData)
        {
            const string ExcelTemplatePath = "ExcelTemplates/JobLevelSummaryReportByUnitAndCountry.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new ManpowerReportExcel(templatePath, gridUnitData, gridCountryData);
            return excel.Export();
        }
    }
}