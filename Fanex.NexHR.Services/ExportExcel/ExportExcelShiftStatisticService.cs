namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;

    public interface IExportExcelShiftStatisticService
    {
        byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails);
    }

    public class ExportExcelShiftStatisticService : IExportExcelShiftStatisticService
    {
        public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
        {
            const int FirstDataTableRow = 3;
            const int TotalDataTableCol = 6;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ShiftStatisticDetailTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                int rowIndex = FirstDataTableRow;
                int colIndex = 1;
                int count = 1;

                var oddRowTableStyle = workSheet.Cells[FirstDataTableRow, 1, FirstDataTableRow, TotalDataTableCol];
                var evenRowTableStyle = workSheet.Cells[FirstDataTableRow + 1, 1, FirstDataTableRow + 1, TotalDataTableCol];

                foreach (var detail in shiftDetails)
                {
                    //Copy row style
                    var rowTableStyle = rowIndex % 2 != 0 ? oddRowTableStyle : evenRowTableStyle;
                    rowTableStyle.Copy(workSheet.Cells[rowIndex, 1, rowIndex, TotalDataTableCol]);

                    //Fill data
                    workSheet.Cells[rowIndex, colIndex++].Value = count;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.WorkDate;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Section;
                    workSheet.Cells[rowIndex, colIndex].Value = detail.Shift;

                    rowIndex++;
                    colIndex = 1;
                    count++;
                }

                return excelPackage.GetAsByteArray();
            }
        }
    }
}