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

        public byte[] ExportAnnualManpowerForecastRequestForm(AnnualManpowerForecastRequest request)
        {
            const string ExcelTemplatePath = "ExcelTemplates/AnnualManpowerForecastRequestForm.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new AnnualManpowerForecastRequestFormExcel(templatePath, request);
            return excel.Export();
        }

        public byte[] ExportJobLevelSummaryReport(GridData gridUnitData, GridData gridCountryData)
        {
            const string ExcelTemplatePath = "ExcelTemplates/JobLevelSummaryReportByUnitAndCountry.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new JobLevelSummaryReportExcel(templatePath, gridUnitData, gridCountryData);
            return excel.Export();
        }

        public byte[] ExportManpowerForecastReport(IEnumerable<AnnualManpowerForecastRequest> requests, int year, string periodName)
        {
            const string ExcelTemplatePath = "ExcelTemplates/ManpowerForecast.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new ManpowerForecastReportExcel(templatePath, requests, year, periodName);
            return excel.Export();
        }
    }
}