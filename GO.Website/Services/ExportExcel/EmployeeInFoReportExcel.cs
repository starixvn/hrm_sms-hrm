namespace GO.Website.Services.ExportExcel
{
    using System.Collections.Generic;
    using Models.Manpower;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class EmployeeInFoReportExcel : ExcelBase
    {
        private readonly IEnumerable<EmployeeInfo> employeeInfos;

        private const byte RowHeaderIndex = 3;
        private const byte RowBodyIndex = 4;
        private const byte NumCol = 13;
        private const byte ColCompany = 2;
        private const byte ColCategory = 8;
        private const byte ColDepartment = 10;
        private const byte SizeColMiddle = 15;
        private const string Local = "Local";
        private const string Expat = "Expat";
        private const string Yes = "Yes";
        private const string No = "No";
        public EmployeeInFoReportExcel(
            string templatePath,
            IEnumerable<EmployeeInfo> employeeInfos) : base(templatePath, 0, "Employee Information")
        {
            this.employeeInfos = employeeInfos;
        }

        protected internal override void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[1, 1, 1, NumCol].Copy(worksheet.Cells[1, 1, 1, NumCol]);
        }

        protected internal override void ProcessUI(ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns();
            worksheet.Column(ColCompany).Width = SizeColMiddle;
            worksheet.Column(ColCategory).Width = SizeColMiddle;
            worksheet.Column(ColDepartment).Width = SizeColMiddle;
        }

        protected internal override void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            templateWorksheet.Cells[RowHeaderIndex, 1, RowHeaderIndex, NumCol].Copy(worksheet.Cells[RowHeaderIndex, 1, RowHeaderIndex, NumCol]);
        }

        protected internal override void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet)
        {
            int rowIndex = RowBodyIndex;
            var colIndex = 1;
            var numRow = 1;
            foreach (var item in employeeInfos)
            {
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = numRow++;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.CompanyName;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.Username;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.Code;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value = item.FullName;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.JobLevel;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex++].Value = item.IsLocal ? Local : Expat;
                worksheet.Cells[rowIndex, colIndex++].Value = item.UnitName;
                worksheet.Cells[rowIndex, colIndex++].Value = item.DepartmentName;
                worksheet.Cells[rowIndex, colIndex++].Value = item.SectionName;
                worksheet.Cells[rowIndex, colIndex++].Value = item.TeamName;
                worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, colIndex].Value = !item.IsExcluded ? Yes : No;
                rowIndex++;
                colIndex = 1;
            }
        }
    }
}
