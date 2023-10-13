namespace GO.Website.UnitTests.Services.ExportExcel
{
    using System.IO;
    using OfficeOpenXml;

    public class FakeExcelPackage
    {
        public FakeExcelPackage()
        {
            if (!Directory.Exists("ExcelFile"))
            {
                Directory.CreateDirectory("ExcelFile");
            }
        }

        public ExcelPackage ExcelPackage => new ExcelPackage(new FileInfo("ExcelFile\\Worksheet.xlsx"));

        public ExcelWorksheets Worksheets => ExcelPackage.Workbook.Worksheets;
    }
}
