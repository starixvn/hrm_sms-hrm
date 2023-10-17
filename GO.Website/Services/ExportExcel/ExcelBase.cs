namespace GO.Website.Services
{
    using System.Drawing;
    using System.IO;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public abstract class ExcelBase
    {
        private readonly short _templateSheetIndex;
        private readonly string _templatePath;
        private readonly string _sheetName;
        protected static readonly Color HeaderTitleColor = Color.FromArgb(60, 94, 165);
        protected static readonly Color EvenRowColor = Color.FromArgb(228, 242, 253);

        protected ExcelBase(string templatePath, short templateSheetIndex = 0, string sheetName = "Data")
        {
            _templatePath = templatePath;
            _templateSheetIndex = templateSheetIndex;
            _sheetName = sheetName;
        }

        protected internal abstract void RenderTitle(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet);

        protected internal abstract void RenderHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet);

        protected internal abstract void RenderBody(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet);

        protected internal virtual void ProcessUI(ExcelWorksheet worksheet)
        {
        }

        public virtual byte[] Export()
        {
            using (var templateExcelPackage = new ExcelPackage(new FileInfo(_templatePath)))
            {
                using (var excelPackage = new ExcelPackage())
                {
                    var templateWorksheet = templateExcelPackage.Workbook.Worksheets[_templateSheetIndex];
                    var worksheet = excelPackage.Workbook.Worksheets.Add(_sheetName);

                    RenderTitle(templateWorksheet, worksheet);
                    RenderHeader(templateWorksheet, worksheet);
                    RenderBody(templateWorksheet, worksheet);

                    ProcessUI(worksheet);

                    return excelPackage.GetAsByteArray();
                }
            }
        }
    }
}