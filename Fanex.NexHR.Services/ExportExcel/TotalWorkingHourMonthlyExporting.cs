namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public class TotalWorkingHourMonthlyExporting : ITotalWorkingHourMonthlyExporting
    {
        private const int _RowIndexOfHeaderTitle = 1;
        private const int _RowIndexOfTablePosition = 3;
        private readonly ICellStyle _evenCellStyleWithLeftAlign;
        private readonly ICellStyle _oddCellStyleWithLeftAlign;
        private readonly ICellStyle _evenCellStyleWithCenterAlign;
        private readonly ICellStyle _oddCellStyleWithCenterAlign;
        private readonly ICellStyle _defaultHeaderCellStyle;
        private readonly ICellStyle _offDayHeaderCellStyle;
        private IList<TotalWorkingHourMonthly> _totalWorkingHourMonthly;
        private TotalWorkingHourMonthly _firstRecord;
        private HSSFWorkbook _workbook;
        private ISheet _sheet;

        public TotalWorkingHourMonthlyExporting(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly, TotalWorkingHourMonthly firstRecord)
        {
            this._totalWorkingHourMonthly = totalWorkingHourMonthly;
            this._firstRecord = firstRecord;
            this._workbook = new HSSFWorkbook();
            this._evenCellStyleWithLeftAlign = this.CreateEvenCellStyleWithLeftAlign();
            this._oddCellStyleWithLeftAlign = this.CreateOddCellStyleWithLeftAlign();
            this._evenCellStyleWithCenterAlign = this.CreateEvenCellStyleWithCenterAlign();
            this._oddCellStyleWithCenterAlign = this.CreateOddCellStyleWithCenterAlign();
            this._defaultHeaderCellStyle = this.CreateDefaultHeaderCellStyle();
            this._offDayHeaderCellStyle = this.CreateOffDayHeaderCellStyle();
        }

        public HSSFWorkbook ExportTotalWorkingHourMonthly()
        {
            this.CreateSheet(Labels.TotalWorkingHourMonthly);
            this.ExportHeaderTitle(Labels.TotalWorkingHourMonthly);
            if (this.IsHaveTableHeader(this._totalWorkingHourMonthly))
            {
                this.ExportTable();
            }

            return this._workbook;
        }

        public HSSFWorkbook ExportTotalWorkingHourDaily()
        {
            this.CreateSheet(Labels.TotalWorkingHourDaily);
            this.ExportHeaderTitle(Labels.TotalWorkingHourDaily);
            if (this.IsHaveTableHeader(this._totalWorkingHourMonthly))
            {
                this.ExportTable();
            }

            return this._workbook;
        }

        private void CreateSheet(string sheetName)
        {
            _sheet = this._workbook.CreateSheet(sheetName);
        }

        private void ExportHeaderTitle(string title)
        {
            IRow row = _sheet.CreateRow(_RowIndexOfHeaderTitle);

            IFont font = this._workbook.CreateFont();
            font.FontHeightInPoints = 16;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = row.CreateCell(1);
            cell.SetCellValue(title);
            cell.CellStyle = this._workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            _sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 4));
        }

        private bool IsHaveTableHeader(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly != null && totalWorkingHourMonthly.Any();
        }

        private void ExportTable()
        {
            int rowIndex = _RowIndexOfTablePosition;
            this.ExportHeader(rowIndex);

            if (this.IsHaveContentRecords(_totalWorkingHourMonthly))
            {
                rowIndex++;
                this.ExportContent(rowIndex);
            }

            this.CreateFreezePane();
            this.SetColumnWidthsForTable();
        }

        private void SetColumnWidthsForTable()
        {
            for (var i = 0; i < _sheet.GetRow(_RowIndexOfTablePosition).LastCellNum; i++)
            {
                if (i <= 3)
                {
                    _sheet.AutoSizeColumn(i);
                }
                else if (i == 4)
                {
                    _sheet.SetColumnWidth(i, 12 * 256);
                }
                else
                {
                    _sheet.SetColumnWidth(i, 15 * 256);
                }
            }
        }

        private void CreateFreezePane()
        {
            _sheet.CreateFreezePane(5, 4);
        }

        private void ExportContent(int rowIndex)
        {
            this.RemoveHeaderRecord();
            foreach (var item in _totalWorkingHourMonthly)
            {
                this.ExportOneRowIntoContentRegion(rowIndex, item);
                rowIndex++;
            }
        }

        private ICellStyle CreateOddCellStyleWithLeftAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            return cellStyle;
        }

        private ICellStyle CreateEvenCellStyleWithLeftAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Left;
            return cellStyle;
        }

        private void ExportOneRowIntoContentRegion(int rowIndex, TotalWorkingHourMonthly totalWorkingHourMonthly)
        {
            IRow row = _sheet.CreateRow(rowIndex);
            this.CreateCellContentWithLeftAlign(row, rowIndex, 0, totalWorkingHourMonthly.StaffId.Trim());
            this.CreateCellContentWithLeftAlign(row, rowIndex, 1, totalWorkingHourMonthly.Section.Trim());
            this.CreateCellContentWithLeftAlign(row, rowIndex, 2, totalWorkingHourMonthly.FullNameNick.Trim());
            this.CreateCellContentWithLeftAlign(row, rowIndex, 3, totalWorkingHourMonthly.Nick.Trim());
            this.CreateCellContentWithCenterAlign(row, rowIndex, 4, totalWorkingHourMonthly.CountWorkDay.ToString("0.0"));

            for (int i = 0; i < totalWorkingHourMonthly.WorkingHours.Count; i++)
            {
                this.CreateCellContentWithCenterAlign(row, rowIndex, 5 + i, totalWorkingHourMonthly.WorkingHours[i]);
            }
        }

        private void CreateCellContentWithLeftAlign(IRow row, int rowIndex, int columnIndex, string content)
        {
            ICell cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = rowIndex % 2 == 0 ? _oddCellStyleWithLeftAlign : _evenCellStyleWithLeftAlign;
            cellBody.SetCellValue(content);
        }

        private void CreateCellContentWithCenterAlign(IRow row, int rowIndex, int columnIndex, string content)
        {
            ICell cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = rowIndex % 2 == 0 ? _oddCellStyleWithCenterAlign : _evenCellStyleWithCenterAlign;
            cellBody.SetCellValue(content);
        }

        private void RemoveHeaderRecord()
        {
            _totalWorkingHourMonthly.RemoveAt(0);
        }

        private ICellStyle CreateOddCellStyleWithCenterAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
            return cellStyle;
        }

        private ICellStyle CreateEvenCellStyleWithCenterAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            return cellStyle;
        }

        private bool IsHaveContentRecords(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly.Any();
        }

        private void ExportHeader(int rowIndex)
        {
            IRow row = _sheet.CreateRow(rowIndex);
            this.CreateFreezeHeaderCell(row, 0, Labels.StaffId);
            this.CreateFreezeHeaderCell(row, 1, Labels.Section);
            this.CreateFreezeHeaderCell(row, 2, Labels.FullName);
            this.CreateFreezeHeaderCell(row, 3, Labels.Nick);
            this.CreateFreezeHeaderCell(row, 4, Labels.CountWorkDay);

            for (int i = 0; i < _firstRecord.WorkingHours.Count; i++)
            {
                this.CreateUnfreezeHeaderCell(row, 5 + i, _firstRecord.WorkingHours[i]);
            }
        }

        private void CreateFreezeHeaderCell(IRow row, int columnIndex, string content)
        {
            ICell cell = row.CreateCell(columnIndex);
            cell.CellStyle = _defaultHeaderCellStyle;
            cell.SetCellValue(content);
        }

        private void CreateUnfreezeHeaderCell(IRow row, int columnIndex, string content)
        {
            string currentHeaderDate = content.Split('(')[1].Split(')')[0];
            ICell cell = row.CreateCell(columnIndex);
            cell.CellStyle = _defaultHeaderCellStyle;
            cell.SetCellValue(content);

            if (currentHeaderDate == "Sat" || currentHeaderDate == "Sun")
            {
                cell.CellStyle = _offDayHeaderCellStyle;
            }
        }

        private ICellStyle CreateDefaultHeaderCellStyle()
        {
            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.WrapText = true;
            cellStyle.SetFont(font);

            return cellStyle;
        }

        private ICellStyle CreateOffDayHeaderCellStyle()
        {
            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.Red.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.WrapText = true;
            cellStyle.SetFont(font);

            return cellStyle;
        }
    }
}