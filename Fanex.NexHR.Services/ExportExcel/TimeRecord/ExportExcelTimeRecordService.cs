namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using OfficeOpenXml;

    public interface IExportExcelTimeRecordService
    {
        byte[] Export(GetTimeRecordCriteria criteria, IList<TimeRecordModel> timeRecords);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelTimeRecordService : IExportExcelTimeRecordService
    {
        public byte[] Export(GetTimeRecordCriteria criteria, IList<TimeRecordModel> timeRecords)
        {
            const int FirstDataTableRow = 4;
            const int TotalDataTableCol = 6;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/TimeRecordTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                int rowIndex = FirstDataTableRow;
                int colIndex = 1;
                int count = 1;

                var oddRowTableStyle = workSheet.Cells[FirstDataTableRow, 1, FirstDataTableRow, TotalDataTableCol];
                var evenRowTableStyle = workSheet.Cells[FirstDataTableRow + 1, 1, FirstDataTableRow + 1, TotalDataTableCol];

                foreach (var detail in timeRecords)
                {
                    //Copy row style
                    var rowTableStyle = rowIndex % 2 != 0 ? oddRowTableStyle : evenRowTableStyle;
                    rowTableStyle.Copy(workSheet.Cells[rowIndex, 1, rowIndex, TotalDataTableCol]);

                    //Fill data
                    workSheet.Cells[rowIndex, colIndex++].Value = count;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Code;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Section;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.RecordTime.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + detail.RecordTime.ToString(@"mm\'");
                    workSheet.Cells[rowIndex, colIndex].Value = detail.GetInOutName;

                    rowIndex++;
                    colIndex = 1;
                    count++;
                }

                return excelPackage.GetAsByteArray();
            }
        }
    }
}