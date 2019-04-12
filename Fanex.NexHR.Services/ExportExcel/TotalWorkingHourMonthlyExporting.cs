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
        private readonly IList<TotalWorkingHourMonthly> _totalWorkingHourMonthly;
        private readonly TotalWorkingHourMonthly _firstRecord;
        private readonly HSSFWorkbook _workbook;
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
            CreateSheet(Labels.TotalWorkingHourMonthly);
            ExportHeaderTitle(Labels.TotalWorkingHourMonthly);
            if (IsHaveTableHeader(_totalWorkingHourMonthly))
            {
                ExportTable();
            }

            return _workbook;
        }

        public HSSFWorkbook ExportTotalWorkingHourDaily()
        {
            CreateSheet(Labels.TotalWorkingHourDaily);
            ExportHeaderTitle(Labels.TotalWorkingHourDaily);
            if (IsHaveTableHeader(_totalWorkingHourMonthly))
            {
                ExportTable();
            }

            return _workbook;
        }

        private void CreateSheet(string sheetName)
        {
            _sheet = _workbook.CreateSheet(sheetName);
        }

        private void ExportHeaderTitle(string title)
        {
            IRow row = _sheet.CreateRow(_RowIndexOfHeaderTitle);

            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 16;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = row.CreateCell(1);
            cell.SetCellValue(title);
            cell.CellStyle = _workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            _sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 4));
        }

        private static bool IsHaveTableHeader(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly != null && totalWorkingHourMonthly.Any();
        }

        private void ExportTable()
        {
            int rowIndex = _RowIndexOfTablePosition;
            ExportHeader(rowIndex);

            if (IsHaveContentRecords(_totalWorkingHourMonthly))
            {
                rowIndex++;
                ExportContent(rowIndex);
            }

            CreateFreezePane();
            SetColumnWidthsForTable();
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
            _sheet.CreateFreezePane(6, 4);
        }

        private void ExportContent(int rowIndex)
        {
            RemoveHeaderRecord();
            foreach (var item in _totalWorkingHourMonthly)
            {
                ExportOneRowIntoContentRegion(rowIndex, item);
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
            CreateCellContentWithCenterAlign(row, rowIndex, 0, (rowIndex - _RowIndexOfTablePosition).ToString());
            CreateCellContentWithLeftAlign(row, rowIndex, 1, totalWorkingHourMonthly.StaffId.Trim());
            CreateCellContentWithLeftAlign(row, rowIndex, 2, totalWorkingHourMonthly.Section.Trim());
            CreateCellContentWithLeftAlign(row, rowIndex, 3, totalWorkingHourMonthly.FullNameNick.Trim());
            CreateCellContentWithLeftAlign(row, rowIndex, 4, totalWorkingHourMonthly.Nick.Trim());
            CreateCellDecimalContent(row, rowIndex, 5, (double)totalWorkingHourMonthly.CountWorkDay);

            for (int i = 0; i < totalWorkingHourMonthly.WorkingHours.Count; i++)
            {
                CreateCellContentWithCenterAlign(row, rowIndex, 6 + i, totalWorkingHourMonthly.WorkingHours[i]);
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

        private void CreateCellDecimalContent(IRow row, int rowIndex, int columnIndex, double value)
        {
            ICell cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = rowIndex % 2 == 0 ? _oddCellStyleWithCenterAlign : _evenCellStyleWithCenterAlign;
            cellBody.CellStyle.DataFormat = _workbook.CreateDataFormat().GetFormat("#,##0.0");
            cellBody.SetCellValue(value);
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

        private static bool IsHaveContentRecords(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly.Any();
        }

        private void ExportHeader(int rowIndex)
        {
            IRow row = _sheet.CreateRow(rowIndex);
            CreateFreezeHeaderCell(row, 0, Labels.SymbolCount);
            CreateFreezeHeaderCell(row, 1, Labels.Code);
            CreateFreezeHeaderCell(row, 2, Labels.Section);
            CreateFreezeHeaderCell(row, 3, Labels.FullName);
            CreateFreezeHeaderCell(row, 4, Labels.NickName);
            CreateFreezeHeaderCell(row, 5, Labels.CountWorkDay);

            for (int i = 0; i < _firstRecord.WorkingHours.Count; i++)
            {
                CreateUnfreezeHeaderCell(row, 6 + i, _firstRecord.WorkingHours[i]);
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