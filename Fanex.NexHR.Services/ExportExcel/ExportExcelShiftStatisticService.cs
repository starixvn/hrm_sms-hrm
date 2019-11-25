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

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelShiftStatisticService : IExportExcelShiftStatisticService
    {
        public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
        {
            const int NumOfColumns = 6;
            const int FirstDataTableRow = 2;
            const int FirstDataRenderColumn = 2;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ShiftStatisticDetailTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                //Table Data
                workSheet.Cells[FirstDataTableRow + 1, FirstDataRenderColumn].LoadFromCollection(shiftDetails, false);

                for (int i = 1; i <= shiftDetails.Count(); i++)
                {
                    workSheet.Cells[FirstDataTableRow + i, 1].Value = i;

                    if (i % 2 != 0)
                    {
                        var tableRow = workSheet.Cells[FirstDataTableRow + i, 1, FirstDataTableRow + i, NumOfColumns];
                        tableRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tableRow.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ececec"));
                    }
                }

                var modelTable = workSheet.Cells[FirstDataTableRow, 1, FirstDataTableRow + shiftDetails.Count(), NumOfColumns];
                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                return excelPackage.GetAsByteArray();
            }
        }
    }
}