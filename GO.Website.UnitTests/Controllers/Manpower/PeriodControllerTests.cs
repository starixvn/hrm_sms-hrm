namespace GO.Website.UnitTests.Controllers.Manpower
{
    using AutoFixture;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Controllers.Manpower;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower.Period;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PeriodControllerTests
    {
        private IUserControlSession userControlSession;
        private IBaseService baseService;
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var expected = fixture.Create<bool>();

            userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host).Returns(expected);

            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/Period/Index.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsType<bool>(viewResult.Model);
            Assert.Equal(expected, dataResult);
        }

        [Fact]
        public void Detail_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var id = fixture.Create<int>();
            var expected = fixture.Create<bool>();

            userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host).Returns(expected);

            //Act
            var result = controller.Detail(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/Period/Detail.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsType<bool>(viewResult.Model);
            Assert.Equal(expected, dataResult);
        }

        [Fact]
        public async Task GetPeriods_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetPeriodCriteria>();
            var periods = fixture.Create<IEnumerable<ManpowerPeriod>>();
            var processSettings = fixture.Create<IEnumerable<MapnowerPeriodProcessSetting>>();

            baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria)
                .Returns(new ValueTuple<IEnumerable<ManpowerPeriod>, IEnumerable<MapnowerPeriodProcessSetting>>(periods, processSettings));

            //Act
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            var result = await controller.GetPeriods(criteria);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualPeriods = jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value);
            var actualTotalItems = jsonResult.Value.GetType().GetProperty("totalItems").GetValue(jsonResult.Value);
            Assert.Equal(periods, actualPeriods);
            Assert.Equal(criteria.TotalRows, actualTotalItems);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetPeriodCriteriaByIdCriteria>();
            var periods = fixture.Create<IEnumerable<ManpowerPeriod>>();
            var processSettings = fixture.Create<IEnumerable<MapnowerPeriodProcessSetting>>();

            baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria)
                .Returns(new ValueTuple<IEnumerable<ManpowerPeriod>, IEnumerable<MapnowerPeriodProcessSetting>>(periods, processSettings));

            //Act
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            var result = await controller.GetById(criteria);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(periods, jsonResult.Value);
        }

        [Fact]
        public async Task Create_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var command = fixture.Create<CreatePeriodCommand>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act
           
            var result = await controller.Create(command);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Update_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var command = fixture.Create<UpdatePeriodCommand>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act

            var result = await controller.Update(command);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task UpdateStatus_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var command = fixture.Create<UpdateStatusPeriodCommand>();
            userControlSession.UserId = fixture.Create<int>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act

            var result = await controller.UpdateStatus(command);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Delete_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var command = fixture.Create<DeletePeriodCommand>();
            userControlSession.UserId = fixture.Create<int>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);

            //Act

            var result = await controller.Delete(command);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region helper

        private PeriodController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            baseService = Substitute.For<IBaseService>();

            return new PeriodController(
                userControlSession,
                baseService
            );
        }

        #endregion helper
    }
}
