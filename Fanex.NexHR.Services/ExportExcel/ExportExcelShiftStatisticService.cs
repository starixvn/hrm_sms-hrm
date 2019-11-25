namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using Fanex.NexHR.Models.Report;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public interface IExportExcelShiftStatisticService
    {
        byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails);
    }

    public class ExportExcelShiftStatisticService : IExportExcelShiftStatisticService
    {
        public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
        {
            const int NumOfColumns = 6;
            const int FirstDataRow = 3;
            const double CellsMinWidth = 35;
            const double NumbericOrderMinWidth = 10;
            const int TitleFontSize = 15;

            Color tableHeaderBackgroundColor = ColorTranslator.FromHtml("#343c4e");
            Color tableHeaderFontColor = ColorTranslator.FromHtml("#fff");

            using (var excelPackage = new ExcelPackage())
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                workSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Title
                var titleBlock = workSheet.Cells[1, 1, 1, NumOfColumns];
                titleBlock.Merge = true;
                titleBlock.Style.Font.Bold = true;
                titleBlock.Style.Font.Size = TitleFontSize;
                titleBlock.Value = "Employee Shift Detail Statistic";

                var subtitleBlock = workSheet.Cells[2, 1, 2, NumOfColumns];
                subtitleBlock.Merge = true;
                subtitleBlock.Style.Font.Italic = true;

                //Table header
                var headerBlock = workSheet.Cells[3, 1, 3, NumOfColumns];
                headerBlock.Style.Font.Bold = true;
                headerBlock.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerBlock.Style.Fill.BackgroundColor.SetColor(tableHeaderBackgroundColor);
                headerBlock.Style.Font.Color.SetColor(tableHeaderFontColor);
                workSheet.View.FreezePanes(FirstDataRow + 1, 1);

                int col = 1;
                headerBlock[FirstDataRow, col++].Value = "#";
                headerBlock[FirstDataRow, col++].Value = "Date";
                headerBlock[FirstDataRow, col++].Value = "Nickname";
                headerBlock[FirstDataRow, col++].Value = "Full Name";
                headerBlock[FirstDataRow, col++].Value = "Section";
                headerBlock[FirstDataRow, col].Value = "Shift";

                //Table Data
                workSheet.Cells[FirstDataRow + 1, 2].LoadFromCollection(shiftDetails, false, OfficeOpenXml.Table.TableStyles.Medium2);

                for (int i = 1; i <= shiftDetails.Count(); i++)
                {
                    workSheet.Cells[FirstDataRow + i, 1].Value = i;
                    if (i % 2 == 1)
                    {
                        var tableRow = workSheet.Cells[FirstDataRow + i, 1, FirstDataRow + i, NumOfColumns];
                        tableRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tableRow.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ececec"));
                    }
                }

                var modelTable = workSheet.Cells[FirstDataRow, 1, FirstDataRow + shiftDetails.Count(), NumOfColumns];
                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                workSheet.Cells.AutoFitColumns(CellsMinWidth);
                workSheet.Column(1).AutoFit(NumbericOrderMinWidth);
                workSheet.Column(2).Style.Numberformat.Format = "dd MMM, yyyy";

                var leftAlignBlock = workSheet.Cells[FirstDataRow + 1, 4, FirstDataRow + shiftDetails.Count(), 5];
                leftAlignBlock.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                return excelPackage.GetAsByteArray();
            }
        }
    }
}