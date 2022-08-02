namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;
    using OfficeOpenXml;

    public interface IExportDisciplineSummaryPerYearService
    {
        byte[] Export(int fromYear, int toYear, IList<DisciplineSummaryPerYear> employees);
    }

    public class ExportDisciplineSummaryPerYearService : IExportDisciplineSummaryPerYearService
    {
        public byte[] Export(int fromYear, int toYear, IList<DisciplineSummaryPerYear> employees)
        {
            const byte LengthOfServiceColIndex = 8;
            const byte BodyRowIndex = 5;
            const byte TotalColumns = 13;
            const byte MinimumWidth = 5;
            const byte MaximumWidth = 50;
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/DisciplineSummaryPerYear.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("5 Years Discipline Summary");
                    int rowIndex = 1;
                    var colIndex = 1;

                    // title & header
                    template.Cells[rowIndex, colIndex, BodyRowIndex - 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, BodyRowIndex - 1, TotalColumns]);
                    for (var i = toYear; i >= fromYear; i--)
                    {
                        var diff = DateTime.Today.Year - i;
                        worksheet.Cells[BodyRowIndex - 1, LengthOfServiceColIndex + diff].Value = diff == 1 ? "Last Year" : $"Last {diff} Years";
                    }

                    // data
                    var rowNum = 1;
                    rowIndex = BodyRowIndex;
                    foreach (DisciplineSummaryPerYear employee in employees)
                    {
                        colIndex = 1;
                        template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Team;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.IsLocal ? Labels.Local : Labels.Expat;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.LengthOfService;
                        for (var i = toYear; i >= fromYear; i--)
                        {
                            var discipline = employee.Disciplines.Where(x => x.Year == i).Sum(x => x.ApproverSubtractScore);
                            worksheet.Cells[rowIndex, colIndex++].Value = discipline > 0 ? -discipline : 0;
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}