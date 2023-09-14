namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Report.Manpower;
    using OfficeOpenXml;

    public class ManpowerForecastReportExcel : ExcelBase
    {
        private readonly IEnumerable<AnnualManpowerForecastRequest> requests;

        public ManpowerForecastReportExcel(
               string templatePath,
               IEnumerable<AnnualManpowerForecastRequest> requests) : base(templatePath, 0, "ManpowerForecast")
        {
            this.requests = requests;
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            throw new System.NotImplementedException();
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            throw new System.NotImplementedException();
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            throw new System.NotImplementedException();
        }
    }
}