namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;

    public interface IManpowerReportService
    {
        IEnumerable<ManpowerModel> GetManpower(int year);

        IEnumerable<ManpowerCategoryModel> GetManpowerCategory(int year);

        byte[] ExportManpowerCategory(IEnumerable<ManpowerCategoryModel> manpowers, int year);
    }

    public class ManpowerReportService : IManpowerReportService
    {
        private readonly IBaseService baseService;

        public ManpowerReportService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<ManpowerModel> GetManpower(int year)
        {
            var criteria = new GetManpowerCriteria
            {
                Year = year
            };
            return baseService.GetData<ManpowerModel>(criteria);
        }

        public IEnumerable<ManpowerCategoryModel> GetManpowerCategory(int year)
        {
            var criteria = new GetManpowerCategoryCriteria
            {
                Year = year
            };
            return baseService.GetData<ManpowerCategoryModel>(criteria);
        }

        public byte[] ExportManpowerCategory(IEnumerable<ManpowerCategoryModel> manpowers, int year)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ManpowerCategory.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Manpower Category");
                    const byte TotalColumns = 15;
                    const byte TotalHeaderRows = 2;
                    const byte StartBodyRow = 4;
                    const byte StartSummaryRow = 5;
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format(worksheet.Cells[rowIndex, colIndex].Value.ToString(), year);
                    rowIndex++;

                    // header
                    template.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);

                    // data
                    rowIndex += TotalHeaderRows;
                    var rowNum = 1;
                    foreach (ManpowerCategoryModel manpower in manpowers)
                    {
                        colIndex = 1;
                        template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.GroupName;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.TotalLocal;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.Total;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.Management;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGManagement;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.ExecutiveWithPosition;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGExecutiveWithPosition;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.Executive;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGExecutive;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.Staff;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGStaff;
                        worksheet.Cells[rowIndex, colIndex++].Value = manpower.Worker;
                        worksheet.Cells[rowIndex, colIndex].Value = manpower.AVGWorker;

                        rowIndex++;
                    }

                    // summary
                    template.Cells[StartSummaryRow, 1, StartSummaryRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
                    WriteSummaryRow(worksheet, rowIndex, manpowers);

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(StartBodyRow, 1);
                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static void WriteSummaryRow(ExcelWorksheet worksheet, int rowIndex, IEnumerable<ManpowerCategoryModel> manpowers)
        {
            int total = manpowers.Sum(t => t.Total);
            int management = manpowers.Sum(t => t.Management);
            int executiveWithPosition = manpowers.Sum(t => t.ExecutiveWithPosition);
            int executive = manpowers.Sum(t => t.Executive);
            int staff = manpowers.Sum(t => t.Staff);
            int worker = manpowers.Sum(t => t.Worker);

            var colIndex = 3;
            worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(x => x.TotalExpat);
            worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(x => x.TotalLocal);
            worksheet.Cells[rowIndex, colIndex++].Value = total;
            worksheet.Cells[rowIndex, colIndex++].Value = management;
            worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? (management * 1.00) / total : 0;
            worksheet.Cells[rowIndex, colIndex++].Value = executiveWithPosition;
            worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? (executiveWithPosition * 1.00) / total : 0;
            worksheet.Cells[rowIndex, colIndex++].Value = executive;
            worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? (executive * 1.00) / total : 0;
            worksheet.Cells[rowIndex, colIndex++].Value = staff;
            worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? (staff * 1.00) / total : 0;
            worksheet.Cells[rowIndex, colIndex++].Value = worker;
            worksheet.Cells[rowIndex, colIndex].Value = total != 0 ? (worker * 1.00) / total : 0;
        }
    }
}