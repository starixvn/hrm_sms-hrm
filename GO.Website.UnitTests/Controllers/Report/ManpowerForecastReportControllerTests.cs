namespace GO.Website.UnitTests.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Manpower;
    using Models.Report.Manpower;
    using NSubstitute;
    using Website.Controllers.Report;
    using Website.Criteria.Report;
    using Website.Criteria.Report.Manpower;
    using Website.Services;
    using Xunit;

    public class ManpowerForecastReportControllerTests
    {
        private IUserControlSession userControlSession;
        private IManpowerForecastService manpowerForecastService;
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
            Assert.Equal("~/Views/Report/ManpowerForecastReport.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetRequests_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var param = fixture.Create<GetManpowerForecastReportParam>();
            var requests = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();
            manpowerForecastService.GetManpowerForecastReportAsync(Arg.Any<GetManpowerForecastReportCriteria>())
                .Returns(requests);

            //Act
            var result = await controller.GetRequests(param);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(requests, viewResult.Value);
        }

        [Fact]
        public async Task ExportExcel_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var param = fixture.Create<GetManpowerForecastReportParam>();
            var requests = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();
            var expected = fixture.Create<byte[]>();
            var reportName = $"ManpowerForecast_{DateTime.Today:yyyyMMdd}.xlsx";


            manpowerForecastService.GetManpowerForecastReportAsync(Arg.Any<GetManpowerForecastReportCriteria>()).Returns(requests);

            excelService.ExportManpowerForecastReport(Arg.Any<IEnumerable<AnnualManpowerForecastRequest>>(), Arg.Any<int>(), Arg.Any<string>()).Returns(expected);
            //Act
            var result = await controller.ExportExcel(param);

            // Assert
            var excelResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/xlsx", excelResult.ContentType);
            Assert.Equal(reportName, excelResult.FileDownloadName);
        }

        #region helper
        private ManpowerForecastReportController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            manpowerForecastService = Substitute.For<IManpowerForecastService>();
            excelService = Substitute.For<IExportExcelService>();

            return new ManpowerForecastReportController(
                userControlSession,
                manpowerForecastService,
                excelService
            );
        }

        #endregion helper
    }
}
