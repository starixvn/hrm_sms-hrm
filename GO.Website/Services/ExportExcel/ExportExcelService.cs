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

        public byte[] ExportManpowerReport(GridData groupByUnitData, GridData groupByCountryData)
        {
            const string ExcelTemplatePath = "ExcelTemplates/ManpowerReport.xlsx";
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ExcelTemplatePath);
            var excel = new ManpowerReportExcel(templatePath, groupByUnitData, groupByCountryData);
            return excel.Export();
        }
    }
}