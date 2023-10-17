using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO.Website.UnitTests.Services.ExportExcel.TestableClasses
{
    using Models.Report.Manpower;
    using OfficeOpenXml;
    using Website.Services;

    public class JobLevelSummaryReportControllerTestable : JobLevelSummaryReportExcel
    {

        public JobLevelSummaryReportControllerTestable(string templatePath, GridData gridUnitData, GridData gridCountryData)
            : base(templatePath, gridUnitData, gridCountryData)
        {
        }

        public void Public_RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            base.RenderTitle(templateWorksheet, worksheet);
        }

        public void Public_RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            base.RenderHeader(templateWorksheet, worksheet);
        }

        public void Public_RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            base.RenderBody(templateWorksheet, worksheet);
        }

        public void Public_ProcessUI(ExcelWorksheet worksheet)
        {
            base.ProcessUI(worksheet);
        }

    }
}
