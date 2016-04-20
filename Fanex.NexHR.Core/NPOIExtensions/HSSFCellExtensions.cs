namespace Fanex.NexHR.Core.NPOIExtensions
{
    using System;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    public static class HSSFCellExtensions
    {
        public static void SetCellValue(this ICell cell, object value)
        {
            if (cell != null)
            {
                if (value == null)
                {
                    cell.SetCellValue(string.Empty);
                }
                else
                {
                    var stringValue = value as string;
                    var richTextString = value as IRichTextString;

                    if (stringValue != null)
                    {
                        cell.SetCellValue(stringValue);
                    }
                    else if (value is int)
                    {
                        cell.SetCellValue((int)value);
                    }
                    else if (value is long)
                    {
                        cell.SetCellValue((long)value);
                    }
                    else if (value is double)
                    {
                        cell.SetCellValue((double)value);
                    }
                    else if (value is decimal)
                    {
                        cell.SetCellValue((decimal)value);
                    }
                    else if (value is bool)
                    {
                        cell.SetCellValue((bool)value);
                    }
                    else if (value is DateTime)
                    {
                        cell.SetCellValue((DateTime)value);
                    }
                    else if (richTextString != null)
                    {
                        cell.SetCellValue(richTextString);
                    }
                }
            }
        }

        public static void SetCellValue(this ICell cell, double? value)
        {
            if (cell != null)
            {
                value = value ?? 0;
                cell.SetCellValue((double)value);
            }
        }

        public static void SetCellValue(this ICell cell, decimal value)
        {
            if (cell != null)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
        }

        public static void SetCellValue(this ICell cell, decimal? value)
        {
            value = value ?? 0;
            cell.SetCellValue((decimal)value);
        }

        public static void SetCellValue(this ICell cell, int value)
        {
            if (cell != null)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
        }

        public static void SetCellValue(this ICell cell, int? value)
        {
            if (cell != null)
            {
                value = value ?? 0;
                cell.SetCellValue((int)value);
            }
        }

        public static void SetCellValue(this ICell cell, long value)
        {
            if (cell != null)
            {
                cell.SetCellValue(Convert.ToDouble(value));
            }
        }

        public static void SetCellValue(this ICell cell, long? value)
        {
            if (cell != null)
            {
                value = value ?? 0;
                cell.SetCellValue((long)value);
            }
        }

        public static void CopyFrom(this ICell cell, ICell sourceCell)
        {
            // Copy merged cells.
            CellRangeAddress sourceRange = null;

            if (cell != null && sourceCell != null)
            {
                for (var i = 0; i < sourceCell.Sheet.NumMergedRegions; i++)
                {
                    CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(i);

                    if ((range.FirstColumn <= sourceCell.ColumnIndex)
                        && (range.LastColumn >= sourceCell.ColumnIndex)
                        && (range.FirstRow <= sourceCell.RowIndex)
                        && (range.LastRow >= sourceCell.RowIndex))
                    {
                        sourceRange = range;
                        break;
                    }
                }

                if (sourceRange != null)
                {
                    cell.Sheet.AddMergedRegion(sourceRange.Clone());
                }

                // Copy style
                cell.CellStyle = sourceCell.CellStyle;
            }
        }
    }
}