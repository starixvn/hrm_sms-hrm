namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using GO.Website.Models.Report.Manpower;
    using OfficeOpenXml;

    public class ManpowerReportExcel : ExcelBase
    {
        private readonly GridData gridUnitData;
        private readonly GridData gridCountryData;
        private int _startCountryColIndex;
        private const byte RowHeaderIndex = 2;
        private const byte RowBodyIndex = 5;
        private const byte TotalRowIndex = 6;
        private const byte GrandTotalRowIndex = 7;

        public ManpowerReportExcel(
               string templatePath,
               GridData gridUnitData,
               GridData gridCountryData) : base(templatePath, 0, "Summary")
        {
            this.gridUnitData = gridUnitData;
            this.gridCountryData = gridCountryData;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1].Copy(worksheet.Cells[1, 1]);
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            const byte UnitColIndex = 2;
            const byte TotalColIndex = 4;
            const byte CountryColIndex = 8;

            // unit
            var headers = gridUnitData.Headers;
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1].Copy(worksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1]);
            const byte ColStep = 2;
            var colIndex = 2;
            var gunitColIndex = 2;
            foreach (var gunit in headers)
            {
                foreach (var unit in gunit.Cells)
                {
                    templateWorksheet.Cells[RowHeaderIndex, UnitColIndex, RowHeaderIndex, UnitColIndex + 1]
                           .Copy(worksheet.Cells[RowHeaderIndex, colIndex, 1, colIndex + 1]);

                    templateWorksheet.Cells[RowHeaderIndex + 1, UnitColIndex, RowBodyIndex - 1, UnitColIndex + 1]
                        .Copy(worksheet.Cells[RowHeaderIndex + 1, colIndex, RowBodyIndex - 1, colIndex + 1]);
                    worksheet.Cells[RowHeaderIndex + 1, colIndex].Value = unit.Name;

                    colIndex += ColStep;
                }

                templateWorksheet.Cells[RowHeaderIndex, TotalColIndex, RowBodyIndex - 1, TotalColIndex + 1]
                        .Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 1]);

                var excelAddress = $"{worksheet.Cells[RowHeaderIndex, gunitColIndex, RowHeaderIndex, colIndex + 1].Address}";
                worksheet.Cells[excelAddress].Merge = true;
                worksheet.Cells[RowHeaderIndex, gunitColIndex].Value = gunit.Name;

                colIndex += ColStep;
                gunitColIndex = colIndex;
            }

            // country
            colIndex++;
            _startCountryColIndex = colIndex;
            headers = gridCountryData.Headers;
            templateWorksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1].Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex]);
            colIndex++;
            foreach (var country in headers)
            {
                templateWorksheet.Cells[RowHeaderIndex, CountryColIndex, RowBodyIndex - 1, CountryColIndex + 2]
                           .Copy(worksheet.Cells[RowHeaderIndex, colIndex, RowBodyIndex - 1, colIndex + 2]);
                worksheet.Cells[RowHeaderIndex, colIndex].Value = country.Name;
                colIndex += 3;
            }
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            // unit
            int rowIndex = RowBodyIndex;
            IList<RowItem> rows = gridUnitData.Supervisors;
            RenderRows(templateWorksheet, worksheet, ref rowIndex, rows);

            RenderTotalRow(templateWorksheet, worksheet, rowIndex, gridUnitData.TotalSupervisor);
            rowIndex++;

            rows = gridUnitData.Employees;
            RenderRows(templateWorksheet, worksheet, ref rowIndex, rows);

            RenderTotalRow(templateWorksheet, worksheet, rowIndex, gridUnitData.TotalEmployee);
            rowIndex++;

            RenderGrandTotalRow(templateWorksheet, worksheet, rowIndex, gridUnitData.GrandTotal);

            // country
            rowIndex = RowBodyIndex;
            rows = gridCountryData.Supervisors;
            RenderCountryRows(templateWorksheet, worksheet, ref rowIndex, rows);

            RenderCountryTotalRow(templateWorksheet, worksheet, rowIndex, gridCountryData.TotalSupervisor);
            rowIndex++;

            rows = gridCountryData.Employees;
            RenderCountryRows(templateWorksheet, worksheet, ref rowIndex, rows);

            RenderCountryTotalRow(templateWorksheet, worksheet, rowIndex, gridCountryData.TotalEmployee);
            rowIndex++;

            RenderCountryGrandTotalRow(templateWorksheet, worksheet, rowIndex, gridCountryData.GrandTotal);
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            var totalCols = 1;
            var colIndex = 2;
            foreach (var gunit in gridUnitData.Headers)
            {
                totalCols += gunit.Cells.Count * 2;
                for (var i = colIndex; i <= totalCols; i++)
                {
                    worksheet.Column(i).OutlineLevel = 1;
                    worksheet.Column(i).Collapsed = true;
                }

                totalCols += 2;
                colIndex = totalCols + 1;
            }

            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(RowBodyIndex, 1);
        }

        private static void RenderRows(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, IList<RowItem> rows)
        {
            const byte UnitColIndex = 2;
            const byte TotalColIndex = 4;

            foreach (var row in rows)
            {
                var colIndex = 1;
                templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

                foreach (var gunit in row.Cells)
                {
                    var totalExpat = 0;
                    var totalLocal = 0;
                    foreach (var unit in gunit.Cells)
                    {
                        templateWorksheet.Cells[RowBodyIndex, UnitColIndex, RowBodyIndex, UnitColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;

                        totalExpat += unit.TotalExpat;
                        totalLocal += unit.TotalLocal;
                    }

                    templateWorksheet.Cells[RowBodyIndex, TotalColIndex, RowBodyIndex, TotalColIndex + 1]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                    worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                    worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
                }

                rowIndex++;
            }
        }

        private static void RenderTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex, RowItem row)
        {
            const byte UnitColIndex = 2;
            const byte TotalColIndex = 4;

            var colIndex = 1;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

            foreach (var gunit in row.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;
                foreach (var unit in gunit.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, UnitColIndex, TotalRowIndex, UnitColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;

                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[TotalRowIndex, TotalColIndex, TotalRowIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }
        }

        private static void RenderGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex, RowItem row)
        {
            const byte UnitColIndex = 2;
            const byte TotalColIndex = 4;

            var colIndex = 1;
            templateWorksheet.Cells[GrandTotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

            foreach (var gunit in row.Cells)
            {
                var totalExpat = 0;
                var totalLocal = 0;
                foreach (var unit in gunit.Cells)
                {
                    templateWorksheet.Cells[GrandTotalRowIndex, UnitColIndex, GrandTotalRowIndex, UnitColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalExpat;
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.TotalLocal;

                    totalExpat += unit.TotalExpat;
                    totalLocal += unit.TotalLocal;
                }

                templateWorksheet.Cells[GrandTotalRowIndex, TotalColIndex, GrandTotalRowIndex, TotalColIndex + 1]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 1]);
                worksheet.Cells[rowIndex, colIndex++].Value = totalExpat;
                worksheet.Cells[rowIndex, colIndex++].Value = totalLocal;
            }
        }

        private void RenderCountryRows(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, ref int rowIndex, IList<RowItem> rows)
        {
            const byte CountryColIndex = 8;

            foreach (var row in rows)
            {
                var colIndex = _startCountryColIndex;
                templateWorksheet.Cells[RowBodyIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

                foreach (var country in row.Cells)
                {
                    foreach (var unit in country.Cells)
                    {
                        templateWorksheet.Cells[RowBodyIndex, CountryColIndex, RowBodyIndex, CountryColIndex]
                            .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                        worksheet.Cells[rowIndex, colIndex++].Value = unit.Total;
                    }
                }

                rowIndex++;
            }
        }

        private void RenderCountryTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex, RowItem row)
        {
            const byte CountryColIndex = 8;

            var colIndex = _startCountryColIndex;
            templateWorksheet.Cells[TotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

            foreach (var country in row.Cells)
            {
                foreach (var unit in country.Cells)
                {
                    templateWorksheet.Cells[TotalRowIndex, CountryColIndex, TotalRowIndex, CountryColIndex]
                               .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = unit.Total;
                }
            }
        }

        private void RenderCountryGrandTotalRow(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, int rowIndex, RowItem row)
        {
            const byte CountryColIndex = 8;

            var colIndex = _startCountryColIndex;
            templateWorksheet.Cells[GrandTotalRowIndex, 1].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex++].Value = row.Name;

            foreach (var country in row.Cells)
            {
                templateWorksheet.Cells[GrandTotalRowIndex, CountryColIndex, GrandTotalRowIndex, CountryColIndex + 2]
                           .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2]);
                worksheet.Cells[rowIndex, colIndex].Value = country.Cells.First().Total;
                colIndex += 3;
            }
        }
    }
}