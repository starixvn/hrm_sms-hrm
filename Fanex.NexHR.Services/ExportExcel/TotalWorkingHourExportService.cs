namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
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
            var rowTitle = sheet.CreateRow(0);
            var titleString = "Total Working Hours: " + fromDate.ToString(Constants.DateFormat) + " - " + toDate.ToString(Constants.DateFormat);

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            var cell = rowTitle.CreateCell(0);
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 24));
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.SetFont(font);

            WriteHeader(workbook, sheet);

            var rowIndex = 5;

            ICellStyle bodyStyle;
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
                bodyStyle = (rowIndex % 2 == 0) ? bodyStyle2 : bodyStyle1;

                IRow row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 4);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHour.Employee.Code.Trim());

                bodyStyle3.Alignment = HorizontalAlignment.Left;
                cellBody = row.CreateCell(2);
                cellBody.CellStyle = (rowIndex % 2 == 0) ? bodyStyle4 : bodyStyle3;
                cellBody.SetCellValue(totalWorkingHour.Employee.FullName);

                WriteCell(row.CreateCell(3), bodyStyle, totalWorkingHour.Employee.NickName);
                WriteCell(row.CreateCell(4), bodyStyle, totalWorkingHour.Employee.Team.Name);
                WriteCell(row.CreateCell(5), bodyStyle, totalWorkingHour.WorkingDay);
                WriteCell(row.CreateCell(6), bodyStyle, totalWorkingHour.WorkingHours);
                WriteCell(row.CreateCell(7), bodyStyle, totalWorkingHour.TotalHours.ToString("0.00"));
                WriteCell(row.CreateCell(8), bodyStyle, totalWorkingHour.ActualWorkingDay);
                WriteCell(row.CreateCell(9), bodyStyle, totalWorkingHour.PaymentWorkingDay);

                WriteCell(row.CreateCell(10), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.AnnualLeave));
                WriteCell(row.CreateCell(11), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.BusinessTrip));
                WriteCell(row.CreateCell(12), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.BereavementLeave));
                WriteCell(row.CreateCell(13), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.Compensationleave));
                WriteCell(row.CreateCell(14), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.WeddingLeave));
                WriteCell(row.CreateCell(15), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.TotalPaymentWorkingDay));
                WriteCell(row.CreateCell(16), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.UnpaidLeave));
                WriteCell(row.CreateCell(17), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.TotalUnpaidLeaveInsurance));
                WriteCell(row.CreateCell(18), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.PeternityLeave));
                WriteCell(row.CreateCell(19), bodyStyle, Utilities.DisplayEmptyFloatNumber(totalWorkingHour.MaternityLeave));

                WriteCell(row.CreateCell(20), bodyStyle, Utilities.DisplayFloatNumber(totalWorkingHour.TotalUnpaidLeaveInsurance));
                WriteCell(row.CreateCell(21), bodyStyle, Utilities.DisplayFloatNumber(totalWorkingHour.OTNormalHours));
                WriteCell(row.CreateCell(22), bodyStyle, Utilities.DisplayFloatNumber(totalWorkingHour.OTOffHours));
                WriteCell(row.CreateCell(23), bodyStyle, Utilities.DisplayFloatNumber(totalWorkingHour.OTPublicHours));
                WriteCell(row.CreateCell(24), bodyStyle, string.Empty);

                rowIndex++;
            }

            sheet.CreateFreezePane(0, 5);

            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return workbook;
        }

        public static void WriteCell(ICell cell, ICellStyle cellStyle, dynamic value)
        {
            cell.CellStyle = cellStyle;
            cell.SetCellValue(value);
        }

        public static void WriteHeader(HSSFWorkbook workbook, ISheet sheet)
        {
            var rowIndex = 3;

            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Color = HSSFColor.Grey80Percent.White.Index;
            font1.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.TopBorderColor = 1;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.RightBorderColor = 1;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.LeftBorderColor = 1;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BottomBorderColor = 1;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font1);

            //Write Header 1
            var row = sheet.CreateRow(rowIndex);

            var cellEmpInfo = row.CreateCell(0);
            cellEmpInfo.CellStyle = cellStyle;
            cellEmpInfo.SetCellValue("Employee Info");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 4));

            var cellContract = row.CreateCell(5);
            cellContract.CellStyle = cellStyle;
            cellContract.SetCellValue("Contract Working Agreement");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 5, 6));

            var cellActualWork = row.CreateCell(7);
            cellActualWork.CellStyle = cellStyle;
            cellActualWork.SetCellValue("Actual Working Time in/out");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 7, 8));

            var cellPayment = row.CreateCell(9);
            cellPayment.CellStyle = cellStyle;
            cellPayment.SetCellValue("Total Working Days for Salary Payment");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 9, 15));

            var cellUnpaid = row.CreateCell(16);
            cellUnpaid.CellStyle = cellStyle;
            cellUnpaid.SetCellValue("Unpaid Leave");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 16, 20));

            var cellOvertime = row.CreateCell(21);
            cellOvertime.CellStyle = cellStyle;
            cellOvertime.SetCellValue("Overtime");
            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 21, 24));
            rowIndex++;

            //Write Header 2
            row = sheet.CreateRow(rowIndex);

            var titleArray = new[]
            {
                Labels.SymbolCount, Labels.Code, Labels.FullName, Labels.NickName, Labels.Section,
                Labels.WorkingDaysMonth, Labels.HoursPerDay,
                Labels.TotalOvertimeHours, Labels.ExcelTotalDays,
                Labels.WorkingDays, Labels.AL, Labels.BusinessTrip, Labels.ShortBereavementLeave, Labels.ShortCompensationLeave, Labels.ShortWeddingLeave, Labels.ExcelTotalDays,
                Labels.UPL, Labels.ExcelSick, Labels.Paternity, Labels.Maternity, Labels.ExcelTotalDays,
                Labels.Normal, Labels.Off, Labels.ShortOTPublicHours, Labels.Tax
            };

            for (int i = 0; i < titleArray.Length; i++)
            {
                var currentCell = row.CreateCell(i);
                currentCell.CellStyle = cellStyle;
                currentCell.SetCellValue(titleArray[i]);
            }
        }
    }
}