namespace Fanex.NexHR.Services.ExportExcel
{
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class TotalWorkingHourExportService : IExportService
    {
        private readonly IList<TotalWorkingHours> totalWorkingHours;
        private readonly DateTime fromDate;
        private readonly DateTime toDate;

        public TotalWorkingHourExportService(IList<TotalWorkingHours> totalWorkingHours, DateTime? fromDate, DateTime? toDate)
        {
            this.totalWorkingHours = totalWorkingHours ?? new List<TotalWorkingHours>();
            this.fromDate = fromDate ?? DateTime.Today;
            this.toDate = toDate ?? DateTime.Today;
        }

        public TotalWorkingHourExportService()
            : this(null, null, null)
        { 
        }

        public HSSFWorkbook Export()
        {
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Total Working Hours");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 12));

            var rowIndex = 3;
            var row = sheet.CreateRow(rowIndex);
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Color = HSSFColor.Grey80Percent.White.Index;
            font1.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font1);

            var cell1 = row.CreateCell(0);
            cell1.CellStyle = cellStyle;
            cell1.SetCellValue("No.");

            var cell2 = row.CreateCell(1);
            cell2.CellStyle = cellStyle;
            cell2.SetCellValue(Labels.Code);

            var cell3 = row.CreateCell(2);
            cell3.CellStyle = cellStyle;
            cell3.SetCellValue(Labels.Name);

            var cell4 = row.CreateCell(3);
            cell4.CellStyle = cellStyle;
            cell4.SetCellValue(Labels.Nick);

            var cell5 = row.CreateCell(4);
            cell5.CellStyle = cellStyle;
            cell5.SetCellValue(Labels.Team);

            var cell6 = row.CreateCell(5);
            cell6.CellStyle = cellStyle;
            cell6.SetCellValue(Labels.WorkigDay);

            var cell7 = row.CreateCell(6);
            cell7.CellStyle = cellStyle;
            cell7.SetCellValue(Labels.ActualWorkingDay);

            var cell8 = row.CreateCell(7);
            cell8.CellStyle = cellStyle;
            cell8.SetCellValue("Leave Taken");

            var cell9 = row.CreateCell(8);
            cell9.CellStyle = cellStyle;
            cell9.SetCellValue("Unpaid Leave");

            var cell10 = row.CreateCell(9);
            cell10.CellStyle = cellStyle;
            cell10.SetCellValue(Labels.LateIn);

            var cell11 = row.CreateCell(10);
            cell11.CellStyle = cellStyle;
            cell11.SetCellValue(Labels.EarlyOut);

            var cell12 = row.CreateCell(11);
            cell12.CellStyle = cellStyle;
            cell12.SetCellValue(Labels.TotalHours);

            var cell13 = row.CreateCell(12);
            cell13.CellStyle = cellStyle;
            cell13.SetCellValue("Average Hours");

            rowIndex++;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.Center;
            bodyStyle1.VerticalAlignment = VerticalAlignment.Center;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.Center;
            bodyStyle2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.Left;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            ICell cellBody;

            foreach (var totalWorkingHour in totalWorkingHours)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 3);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.Employee.Code.Trim());

                bodyStyle3.Alignment = HorizontalAlignment.Left;
                cellBody = row.CreateCell(2);
                if (rowIndex % 2 == 0)
                {
                    cellBody.CellStyle = bodyStyle4;
                }
                else
                {
                    cellBody.CellStyle = bodyStyle3;
                }

                cellBody.SetCellValue(totalWorkingHour.Employee.FullName);

                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.Employee.NickName);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.Employee.Team.Name);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.WorkingDay);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.ActualWorkingDay);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.LeaveTaken);

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.UnpaidLeave);

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.LateInMins.ToString("#"));

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.EarlyOutMins.ToString("#"));

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.TotalHours.ToString("0.##"));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.AverageHours.ToString("0.##"));

                rowIndex++;
            }

            sheet.CreateFreezePane(0, 4);

            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return workbook;
        }
    }
}
