namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Models;
    using OfficeOpenXml;

    public interface IExportExcelTimeRecordService
    {
        byte[] Export(IList<TimeRecordModel> timeRecords);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelTimeRecordService : IExportExcelTimeRecordService
    {
        public byte[] Export(IList<TimeRecordModel> timeRecords)
        {
            const int FirstDataTableRow = 1;
            const byte StartBodyRow = 4;
            const byte TotalColumns = 7;


            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/TimeRecordTemplate.xlsx");

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet workSheet = xlPackage.Workbook.Worksheets.Add("Time Record Report");

                    int rowIndex = FirstDataTableRow;
                    int colIndex = 1;
                    int count = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    rowIndex = rowIndex + 2;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);
                    rowIndex++;

                    foreach (var detail in timeRecords)
                    {
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        workSheet.Cells[rowIndex, colIndex++].Value = count;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Code;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Taem;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.RecordTime.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + detail.RecordTime.ToString(@"mm\'");
                        workSheet.Cells[rowIndex, colIndex].Value = detail.GetInOutName;

                        rowIndex++;
                        colIndex = 1;
                        count++;
                    }

                    workSheet.Cells.AutoFitColumns();
                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}