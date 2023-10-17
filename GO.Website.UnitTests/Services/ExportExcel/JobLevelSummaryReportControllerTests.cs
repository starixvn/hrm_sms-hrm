namespace GO.Website.UnitTests.Services.ExportExcel
{
    using AutoFixture;
    using Models.Report.Manpower;
    using NSubstitute;
    using TestableClasses;
    using Xunit;

    public class JobLevelSummaryReportControllerTests
    {
        private readonly FakeExcelPackage excelPackage = new ();
        private GridData gridUnitData;
        private GridData gridCountryData;
        private readonly Fixture fixture = new();
        private const byte RowHeaderIndex = 2;
        private const byte RowBodyIndex = 5;

        [Fact]
        public void RenderTitle_WhenCalled_ExpectedCopyRow()
        {
            //Arrange
            var template = excelPackage.Worksheets.Add("template");
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();
            // Act
            service.Public_RenderTitle(template, worksheet);

            // Assert
            Assert.Equal(template.Cells[1, 1],
                worksheet.Cells[1, 1]);
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
            Assert.Equal(template.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1],
                worksheet.Cells[RowHeaderIndex, 1, RowBodyIndex - 1, 1]);
        }

        [Fact]
        public void RenderBody_WhenCalled_ExpectedCopyRow()
        {
            //Arrange
            var template = excelPackage.Worksheets.Add("template");
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();

            // Act
            service.Public_RenderHeader(template, worksheet);
            service.Public_RenderBody(template, worksheet);

            // Assert
        }


        [Fact]
        public void ProcessUI_WhenCalled_CallExpectedMethod()
        {
            //Arrange
            var worksheet = excelPackage.Worksheets.Add("data");
            var service = CreateExcel();
            // Act
            service.Public_ProcessUI(worksheet);

            // Assert
        }


        #region Helpers

        private JobLevelSummaryReportControllerTestable CreateExcel()
        {
            gridUnitData = fixture.Create<GridData>();
            gridCountryData = fixture.Create<GridData>();
            var excel = Substitute.For<JobLevelSummaryReportControllerTestable>(string.Empty, gridUnitData, gridCountryData);
            return excel;
        }

        #endregion Helpers
    }
}
