namespace GO.Website.UnitTests.Controllers.Manpower
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using GO.Core.Services;
    using GO.Website.Controllers.Report;
    using GO.Website.Criteria.Report;
    using GO.Website.Models.Report.Manpower;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class ManpowerReportControllerTests
    {
        private IBaseService baseService;
        private IExportExcelService excelService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Report/Manpower.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetData_WhenCalled_ReturnExpectedDataEmployeeInfos()
        {
            // Arrange
            var controller = GetController();
            var employeeInfos = fixture.Create<IEnumerable<EmployeeInfo>>();

            baseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetJobLevelSummaryReportCriteria>()).Returns(employeeInfos);
            //Act
            var result = await controller.GetData();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualemployeeInfos = jsonResult.Value.GetType().GetProperty("employeeInfos").GetValue(jsonResult.Value);
            Assert.Equal(employeeInfos, actualemployeeInfos);
        }

        [Fact]
        public async Task GetData_WhenHaveMultipleSupervisor_ReturnExpectedDataGridCountry()
        {
            // Arrange
            var controller = GetController();
            var employeeInfos = new List<EmployeeInfo> {
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "M3-A"
                },
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "M3-A"
                }
            };

            baseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetJobLevelSummaryReportCriteria>()).Returns(employeeInfos);
            //Act
            var result = await controller.GetData();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualGridCountryData = jsonResult.Value.GetType().GetProperty("gridCountryData").GetValue(jsonResult.Value);
            var viewResult = Assert.IsType<GridData>(actualGridCountryData);
            Assert.Equal("M3-A", viewResult.Supervisors.First().Name);
            Assert.Equal("test", viewResult.Supervisors.First().Cells.First().Name);
            Assert.Equal(2, viewResult.Supervisors.First().Cells.First().Cells.First().Total);
            Assert.Equal(2, viewResult.TotalSupervisor.Cells.First().Cells.First().Total);
        }

        [Fact]
        public async Task GetData_WhenHaveMultipleEmployee_ReturnExpectedDataGridUnit()
        {
            // Arrange
            var controller = GetController();
            var employeeInfos = new List<EmployeeInfo> {
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "E3-A"
                },
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "E3-A"
                }
            };

            baseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetJobLevelSummaryReportCriteria>()).Returns(employeeInfos);
            //Act
            var result = await controller.GetData();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualGridUnitData = jsonResult.Value.GetType().GetProperty("gridUnitData").GetValue(jsonResult.Value);
            var viewResult = Assert.IsType<GridData>(actualGridUnitData);
            Assert.Equal("E3-A", viewResult.Employees.First().Name);
            Assert.Equal("PU01", viewResult.Employees.First().Cells.First().Cells.First().Name);
            Assert.Equal(2, viewResult.Employees.First().Cells.First().Cells.First().TotalExpat);
            Assert.Equal("PU01", viewResult.TotalEmployee.Cells.First().Cells.First().Name);
            Assert.Equal(2, viewResult.TotalEmployee.Cells.First().Cells.First().TotalExpat);
        }

        [Fact]
        public async Task GetData_WhenHaveSupervisorAndEmployee_ReturnExpectedDataGridUnitAndGridCountry()
        {
            // Arrange
            var controller = GetController();
            var employeeInfos = new List<EmployeeInfo> {
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "E3-A"
                },
                new EmployeeInfo
                {
                    CompanyName = "Test1",
                    Country = "test",
                    UnitName = "PU01",
                    JobLevel = "M3-A"
                }
            };

            baseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetJobLevelSummaryReportCriteria>()).Returns(employeeInfos);
            //Act
            var result = await controller.GetData();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualGridUnitData = jsonResult.Value.GetType().GetProperty("gridUnitData").GetValue(jsonResult.Value);
            var viewResultUnit = Assert.IsType<GridData>(actualGridUnitData);
            Assert.Equal("E3-A", viewResultUnit.Employees.First().Name);
            Assert.Equal("PU01", viewResultUnit.Employees.First().Cells.First().Cells.First().Name);
            Assert.Equal(1, viewResultUnit.Employees.First().Cells.First().Cells.First().TotalExpat);
            Assert.Equal("PU01", viewResultUnit.TotalEmployee.Cells.First().Cells.First().Name);
            Assert.Equal(1, viewResultUnit.TotalEmployee.Cells.First().Cells.First().TotalExpat);

            var actualGridCountryData = jsonResult.Value.GetType().GetProperty("gridCountryData").GetValue(jsonResult.Value);
            var viewResultCountry = Assert.IsType<GridData>(actualGridCountryData);
            Assert.Equal("M3-A", viewResultCountry.Supervisors.First().Name);
            Assert.Equal("test", viewResultCountry.Supervisors.First().Cells.First().Name);
            Assert.Equal(1, viewResultCountry.Supervisors.First().Cells.First().Cells.First().Total);
            Assert.Equal(1, viewResultCountry.TotalSupervisor.Cells.First().Cells.First().Total);
        }

        [Fact]
        public async Task ExportExcel_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var employeeInfos = fixture.Create<IEnumerable<EmployeeInfo>>();
            var expected = fixture.Create<byte[]>();
            var reportName = $"ManpowerReport_{DateTime.Today:yyyyMMdd}.xlsx";

            baseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetJobLevelSummaryReportCriteria>()).Returns(employeeInfos);
            excelService.ExportJobLevelSummaryReport(Arg.Any<GridData>(), Arg.Any<GridData>()).Returns(expected);
            //Act
            var result = await controller.ExportExcel();

            // Assert
            var excelResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/xlsx", excelResult.ContentType);
            Assert.Equal(reportName, excelResult.FileDownloadName);
        }

        #region helper

        private JobLevelSummaryReportController GetController()
        {
            baseService = Substitute.For<IBaseService>();
            excelService = Substitute.For<IExportExcelService>();

            return new JobLevelSummaryReportController(
                baseService,
                excelService
            );
        }

        #endregion helper
    }
}