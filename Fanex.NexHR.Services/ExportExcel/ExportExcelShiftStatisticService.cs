namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public interface IExportExcelShiftStatisticService
    {
        byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails);
    }

    public class ExportExcelShiftStatisticService : IExportExcelShiftStatisticService
    {
        public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
        {
            const int FirstDataTableRow = 3;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ShiftStatisticDetailTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                int rowIndex = FirstDataTableRow;
                int colIndex = 1;
                int count = 1;

                foreach (var detail in shiftDetails)
                {
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