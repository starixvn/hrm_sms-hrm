namespace GO.Website.UnitTests.Controllers
{
    using AutoFixture;
    using GO.Website.Controllers;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class UnitHeadControllerTests
    {
        private IUnitHeadService unitHeadService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Schedule_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.Schedule();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task GetWeeklySchedule_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetUnitHeadWeeklyScheduleCriteria>();
            var expected = fixture.Create<IEnumerable<UnitHeadWeeklySchedule>>();

            unitHeadService.GetWeeklyScheduleAsync(criteria).Returns(expected);

            //Act
            var result = await controller.GetWeeklySchedule(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region helper

        private UnitHeadController GetController()
        {
            unitHeadService = Substitute.For<IUnitHeadService>();

            return new UnitHeadController(
                unitHeadService
            );
        }

        #endregion helper
    }
}
