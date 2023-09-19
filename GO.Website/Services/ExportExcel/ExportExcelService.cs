namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.IO;
    using GO.Website.Models.Manpower;
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

        public byte[] ExportManpowerForecastReport(IEnumerable<AnnualManpowerForecastRequest> request, int year)
        {
            const string ExcelTemplatePath = "ExcelTemplates/ManpowerForecast.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new ManpowerForecastReportExcel(templatePath, request, year);
            return excel.Export();
        }
    }
}