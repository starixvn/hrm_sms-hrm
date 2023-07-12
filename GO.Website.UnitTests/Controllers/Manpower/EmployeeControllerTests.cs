namespace GO.Website.UnitTests.Controllers.Manpower
{
    using AutoFixture;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Controllers.Manpower;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class EmployeeControllerTests
    {
        private IUserControlSession userControlSession;
        private IBaseService baseService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<bool>();

            userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_EmployeeManagement, Policy.Host).Returns(expected);

            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/Employee/Index.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsType<bool>(viewResult.Model);
            Assert.Equal(expected, dataResult);
        }

        [Fact]
        public async Task GetEmployeeInfos_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetEmployeeCriteria>();
            var expected = fixture.Create<IEnumerable<EmployeeInfo>>();

            baseService.GetDataAsync<EmployeeInfo>(criteria).Returns(expected);

            //Act
            var result = await controller.GetEmployeeInfos(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualdata = jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value);
            var actualTotalItems = jsonResult.Value.GetType().GetProperty("totalItems").GetValue(jsonResult.Value);
            Assert.Equal(expected, actualdata);
            Assert.Equal(criteria.TotalRows, actualTotalItems);
        }

        [Fact]
        public async Task GetCompanies_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<IEnumerable<Company>>();

            baseService.GetDataAsync<Company>(Arg.Any<GetCompanyCriteria>()).Returns(expected);

            //Act
            var result = await controller.GetCompanies();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task UpdateExclude_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<CommandResult>();
            var command = fixture.Create<UpdateExcludedEmployeeCommand>();
            userControlSession.UserId = fixture.Create<int>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act
            var result = await controller.UpdateExclude(command);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region helper

        private EmployeeController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            baseService = Substitute.For<IBaseService>();

            return new EmployeeController(
                userControlSession,
                baseService
            );
        }

        #endregion helper
    }
}
