namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Models.Overtime;
    using OfficeOpenXml;

    public interface IExportOvertimeService
    {
        byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportOvertimeService : IExportOvertimeService
    {
        public byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests)
        {
            const int FirstDataTableRow = 3;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/EmployeeOverTimeTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                int rowIndex = FirstDataTableRow;
                int colIndex = 1;
                int count = 1;

                foreach (var detail in memberRequests)
                {
                    workSheet.Cells[rowIndex, colIndex++].Value = count;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Section;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Month.ToString("MMM-yyyy");
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.NormalHours;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.OffHours;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.PublicHours;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.SpecialHours;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Verifier.Nickname;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Approver.Nickname;
                    workSheet.Cells[rowIndex, colIndex].Value = detail.Status;

                    rowIndex++;
                    colIndex = 1;
                    count++;
                }

                return excelPackage.GetAsByteArray();
            }
        }
    }
}