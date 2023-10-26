namespace GO.Website.UnitTests.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using GO.Website.Models.Manpower;
    using GO.Website.UnitTests.Services.ExportExcel.TestableClasses;
    using NSubstitute;
    using OfficeOpenXml;
    using Xunit;

    public class EmployeeInFoReportExcelTests
    {
        private readonly FakeExcelPackage excelPackage = new ();
        private IEnumerable<EmployeeInfo> employeeInfos;
        private readonly Fixture fixture = new();
        private const byte RowHeaderIndex = 3;
        private const byte RowBodyIndex = 4;
        private const byte NumCol = 13;
        private const byte ColCompany = 2;
        private const byte ColCategory = 8;
        private const byte ColDepartment = 10;
        private const byte SizeColMiddle = 15;

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
            Assert.Equal(template.Cells[1, 1, 1, NumCol],
                worksheet.Cells[1, 1, 1, NumCol]);
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
            Assert.Equal(template.Cells[RowHeaderIndex, 1, RowHeaderIndex, NumCol],
                worksheet.Cells[RowHeaderIndex, 1, RowHeaderIndex, NumCol]);
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
            // Act
            service.Public_ProcessUI(worksheet);

            // Assert
            Assert.Equal(worksheet.Column(ColCompany).Width, SizeColMiddle);
            Assert.Equal(worksheet.Column(ColCategory).Width, SizeColMiddle);
            Assert.Equal(worksheet.Column(ColDepartment).Width, SizeColMiddle);
        }
        #region Helpers

        private EmployeeInFoReportExcelTestable CreateExcel()
        {
            employeeInfos = fixture.Create<IEnumerable<EmployeeInfo>>();

            var excel = Substitute.For<EmployeeInFoReportExcelTestable>(string.Empty, employeeInfos);
            return excel;
        }

        #endregion Helpers
    }
}
