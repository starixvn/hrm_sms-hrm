// --------------------------------------------------------------------------
// <copyright file="TotalWorkingHourMonthlyExporting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Services.ExcelExporting
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// TotalWorkingHourMonthlyExporting Service.
    /// </summary>
    public class TotalWorkingHourMonthlyExporting : ITotalWorkingHourMonthlyExporting
    {
        private const int _RowIndexOfHeaderTitle = 1;
        private const int _RowIndexOfTablePosition = 3;
        private readonly ICellStyle _evenCellStyleWithLeftAlign;
        private readonly ICellStyle _oddCellStyleWithLeftAlign;
        private readonly ICellStyle _evenCellStyleWithCenterAlign;
        private readonly ICellStyle _oddCellStyleWithCenterAlign;
        private readonly ICellStyle _headerCellStyle;
        private readonly ICellStyle _unfreezeHeaderCellStyle;
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
            this._headerCellStyle = this.CreateHeaderCellStyle();
            this._unfreezeHeaderCellStyle = this.CreateHeaderWorkingHourCellStyle();
        }

        /// <summary>
        /// Exports the total working hour monthly.
        /// </summary>
        /// <returns>The instance of HSSFWorkbook supports for exporting excel file.</returns>
        public HSSFWorkbook ExportTotalWorkingHourMonthly()
        {
            this.CreateSheet();
            this.ExportHeaderTitle();
            if (this.IsHaveTableHeader(this._totalWorkingHourMonthly))
            {
                this.ExportTable();
            }

            return this._workbook;
        }

        private void CreateSheet()
        {
            _sheet = this._workbook.CreateSheet(Labels.TotalWorkingHourMonthly);
        }

        private void ExportHeaderTitle()
        {
            var row = _sheet.CreateRow(_RowIndexOfHeaderTitle);
            var font = CreateHeaderTitleFont();
            var cell = row.CreateCell(1);
            cell.SetCellValue(Labels.TotalWorkingHourMonthly);
            cell.CellStyle = this._workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
            _sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 4));
        }

        private IFont CreateHeaderTitleFont()
        {
            var font = this._workbook.CreateFont();
            font.FontHeightInPoints = 14;
            font.Boldweight = (short)FontBoldWeight.BOLD;
            return font;
        }

        private bool IsHaveTableHeader(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly != null && totalWorkingHourMonthly.Count > 0;
        }

        /// <summary>
        /// Exports the table.
        /// </summary>
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

        /// <summary>
        /// Sets the column widths for table.
        /// </summary>
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
            cellStyle.Alignment = HorizontalAlignment.LEFT;
            cellStyle.FillForegroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            return cellStyle;
        }

        private ICellStyle CreateEvenCellStyleWithLeftAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.LEFT;
            return cellStyle;
        }

        /// <summary>
        /// Exports the one row into content region.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="totalWorkingHourMonthly">The totalWorkingHourMonthly.</param>
        private void ExportOneRowIntoContentRegion(int rowIndex, TotalWorkingHourMonthly totalWorkingHourMonthly)
        {
            var row = _sheet.CreateRow(rowIndex);
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
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.FillForegroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            return cellStyle;
        }

        private ICellStyle CreateEvenCellStyleWithCenterAlign()
        {
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            return cellStyle;
        }

        private bool IsHaveContentRecords(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly)
        {
            return totalWorkingHourMonthly.Count > 1;
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        private void ExportHeader(int rowIndex)
        {
            var row = _sheet.CreateRow(rowIndex);
            this.CreateFreezeHeaderCell(row, 0, Labels.StaffId);
            this.CreateFreezeHeaderCell(row, 1, Labels.Section);
            this.CreateFreezeHeaderCell(row, 2, Labels.FullNameNick);
            this.CreateFreezeHeaderCell(row, 3, Labels.Nick);
            this.CreateFreezeHeaderCell(row, 4, Labels.CountWorkDay);

            for (int i = 0; i < _firstRecord.WorkingHours.Count; i++)
            {
                this.CreateUnfreezeHeaderCell(row, 5 + i, _firstRecord.WorkingHours[i]);
            }
        }

        private void CreateFreezeHeaderCell(IRow row, int columnIndex, string content)
        {
            var cell1 = row.CreateCell(columnIndex);
            cell1.CellStyle = _headerCellStyle;
            cell1.SetCellValue(content);
        }

        /// <summary>
        /// Method Create UnFreezeHeaderCell.
        /// </summary>
        /// <param name="row">Row in sheet.</param>
        /// <param name="columnIndex">Column Index.</param>
        /// <param name="content">Content to be filled in column.</param>
        private void CreateUnfreezeHeaderCell(IRow row, int columnIndex, string content)
        {
            string currentHeaderDate = content.Split('(')[1].Split(')')[0];
            var cell1 = row.CreateCell(columnIndex);
            cell1.CellStyle = _unfreezeHeaderCellStyle;
            cell1.SetCellValue(content);

            if (currentHeaderDate == "Sat" || currentHeaderDate == "Sun")
            {
                cell1.CellStyle = _headerCellStyle;
            }
        }

        private IFont CreateHeaderFont()
        {
            var headerFont = _workbook.CreateFont();
            headerFont.FontHeightInPoints = 11;
            headerFont.Boldweight = (short)FontBoldWeight.BOLD;
            headerFont.Color = HSSFColor.WHITE.index;
            return headerFont;
        }

        private IFont CreateHeaderWorkingHourFont()
        {
            var headerWorkingHourFont = _workbook.CreateFont();
            headerWorkingHourFont.FontHeightInPoints = 11;
            headerWorkingHourFont.Boldweight = (short)FontBoldWeight.BOLD;
            headerWorkingHourFont.Color = HSSFColor.BLACK.index;
            return headerWorkingHourFont;
        }

        /// <summary>
        /// Creates the header working hour cell style.
        /// </summary>
        /// <returns>The instance of ICellStyle.</returns>
        private ICellStyle CreateHeaderWorkingHourCellStyle()
        {
            var headerWorkingHourFont = this.CreateHeaderWorkingHourFont();
            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.GREY_40_PERCENT.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = HSSFColor.GREY_40_PERCENT.index;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.WrapText = true;
            cellStyle.SetFont(headerWorkingHourFont);
            return cellStyle;
        }

        /// <summary>
        /// Creates the header cell style.
        /// </summary>
        /// <returns>The instance of ICellStyle.</returns>
        private ICellStyle CreateHeaderCellStyle()
        {
            var headerFont = this.CreateHeaderFont();
            var palette = _workbook.GetCustomPalette();
            var cellStyle = _workbook.CreateCellStyle();
            var headerColor = palette.FindSimilarColor(192, 80, 77);
            cellStyle.FillForegroundColor = headerColor.GetIndex();
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = headerColor.GetIndex();
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.WrapText = true;
            cellStyle.SetFont(headerFont);
            return cellStyle;
        }
    }
}
