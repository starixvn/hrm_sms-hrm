namespace GO.Website.UnitTests.Services.ExportExcel
{
    using System.Linq;
    using AutoFixture;
    using Models.Manpower;
    using NSubstitute;
    using TestableClasses;
    using Xunit;

    public class AnnualManpowerForecastRequestFormExcelTests
    {
        private readonly FakeExcelPackage excelPackage = new ();
        private AnnualManpowerForecastRequest request;
        private readonly Fixture fixture = new();
        private const byte HeaderRowIndex = 3;
        private const byte BodyRowIndex = 6;
        private const byte DeptColIndex = 2;
        private const byte SectionColIndex = 3;
        private const byte StartCompanyColIndex = 7;

        [Fact]
        public void RenderTitle_WhenCalled_ExpectedCopyRow()
        {
            //Arrange
            var template = excelPackage.Worksheets.Add("template");
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();
            const byte TotalCols = 18;
            // Act
            service.Public_RenderTitle(template, worksheet);

            // Assert
            Assert.Equal(template.Cells[1, 1, HeaderRowIndex - 1, TotalCols],
                worksheet.Cells[1, 1, HeaderRowIndex - 1, TotalCols]);
        }

        [Fact]
        public void RenderHeader_WhenCalled_ExpectedCopyRow()
        {
            //Arrange
            var template = excelPackage.Worksheets.Add("template");
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();

            // Act
            service.Public_RenderHeader(template, worksheet);

            // Assert
            Assert.Equal(template.Cells[HeaderRowIndex, 1, BodyRowIndex - 1, StartCompanyColIndex - 1],
                worksheet.Cells[HeaderRowIndex, 1, BodyRowIndex - 1, StartCompanyColIndex - 1]);
        }


        [Fact]
        public void RenderBody_WhenCalled_ExpectedCopyRow()
        {
            //Arrange
            var template = excelPackage.Worksheets.Add("template");
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();

            // Act
            service.Public_RenderBody(template, worksheet);

            // Assert
        }

        [Fact]
        public void ProcessUI_WhenCalled_CallExpectedMethod()
        {
            //Arrange
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();
            const byte MaximumWidth = 20;
            // Act
            service.Public_ProcessUI(worksheet);

            // Assert
            Assert.Equal(worksheet.Column(1).Width, MaximumWidth);
            Assert.Equal(worksheet.Column(DeptColIndex).Width, MaximumWidth);
            Assert.Equal(worksheet.Column(SectionColIndex).Width, MaximumWidth);
        }

        #region Helpers

        private AnnualManpowerForecastRequestFormExcelTestable CreateExcel()
        {
            request = fixture.Create<AnnualManpowerForecastRequest>();
            foreach (var detail in request.Details) {
                detail.CompanyName = "CTI";
            }
            request.Details.Last().CompanyName = "SMS";

            var excel = Substitute.For<AnnualManpowerForecastRequestFormExcelTestable>(string.Empty, request);
            return excel;
        }

        #endregion Helpers
    }
}
