namespace GO.Website.UnitTests.Controllers.API
{
    using GO.Website.Controllers.API;
    using GO.Website.Hubs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using NSubstitute;
    using System.Threading.Tasks;
    using Xunit;

    public class TodolistControllerTests
    {
        private IHubContext<TodolistHub> hubContext;

        [Fact]
        public async Task NotiRefreshHrm_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();

            //Act
            var result = await controller.NotiRefreshHrm();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        #region helper

        private TodolistController GetController()
        {
            hubContext = Substitute.For<IHubContext<TodolistHub>>();

            return new TodolistController(
                hubContext
            );
        }

        #endregion helper
    }
}
