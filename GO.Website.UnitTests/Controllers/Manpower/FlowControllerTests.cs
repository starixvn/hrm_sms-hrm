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

namespace GO.Website.UnitTests.Controllers.Manpower
{
    public class FlowControllerTests
    {
        private IUserControlSession userControlSession;
        private IBaseService baseService;
        private readonly Fixture fixture = new();

        [Fact]
        public void GetViewIndex_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<bool>();

            userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_FlowManagement, Policy.Host).Returns(expected);

            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/Flow/Index.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsType<bool>(viewResult.Model);
            Assert.Equal(expected, dataResult);
        }

        [Fact]
        public async Task GetUnits_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<IEnumerable<Unit>>();

            baseService.GetDataAsync<Unit>(Arg.Any<GetUnitCriteria>()).Returns(expected);

            //Act
            var result = await controller.GetUnits();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetFlows_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<IEnumerable<ManpowerFlow>>();

            baseService.GetDataAsync<ManpowerFlow>(Arg.Any<GetFlowCriteria>()).Returns(expected);

            //Act
            var result = await controller.GetFlows();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetByUnit_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<IEnumerable<ManpowerFlow>>();
            var criteria = fixture.Create<GetFlowByUnitCriteria>();

            baseService.GetDataAsync<ManpowerFlow>(criteria).Returns(expected);

            //Act
            var result = await controller.GetByUnit(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Update_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<CommandResult>();
            var param = fixture.Create<UpdateFlowParam>();

            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateFlowCommand>()).Returns(expected);

            //Act
            var result = await controller.Update(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Update_WhenCalledFlowsNUll_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<CommandResult>();
            var param = fixture.Create<UpdateFlowParam>();
            param.Flows = null;

            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateFlowCommand>()).Returns(expected);

            //Act
            var result = await controller.Update(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<CommandResult>();
            var command = fixture.Create<DeleteFlowCommand>();
            userControlSession.UserId = fixture.Create<int>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act
            var result = await controller.Delete(command);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region helper

        private FlowController GetController()
        {
            baseService = Substitute.For<IBaseService>();
            userControlSession = Substitute.For<IUserControlSession>();

            return new FlowController(
                userControlSession,
                baseService
            );
        }

        #endregion helper
    }
}
