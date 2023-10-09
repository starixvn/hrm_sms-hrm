namespace GO.Website.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Models;
    using GO.Core.Services;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models.Account;
    using NSubstitute;
    using Website.Criteria.Account;
    using Xunit;

    public class UserOtpServiceTests
    {
        private IBaseService baseService;
        private readonly Fixture fixture = new();

        [Fact]
        public async Task UpdateSecretKeyAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetUserOtpService();
            var userId = fixture.Create<int>();
            var siteId = fixture.Create<short>();
            var secretKey = fixture.Create<string>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSecretKeyCommand>()).Returns(expected);
            //Act
            var actual = await service.UpdateSecretKeyAsync(userId, siteId, secretKey);

            //Assert
            Assert.Equal(expected.IsSuccessful, actual);
        }

        [Fact]
        public async Task UpdateOTPStatusAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetUserOtpService();
            var command = fixture.Create<UpdateStatusUserOTPCommand>();
            var expected = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(command).Returns(expected);
            //Act
            var actual = await service.UpdateOTPStatusAsync(command);

            //Assert
            Assert.Equal(expected.IsSuccessful, actual);
        }

        [Fact]
        public async Task GetUserOTPByIdAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetUserOtpService();
            var userId = fixture.Create<int>();
            var siteId = fixture.Create<short>();
            var expected = fixture.Create<UserOtp>();

            baseService.GetDataEntityAsync<UserOtp>(Arg.Any<GetUserOTPByIdCriteria>()).Returns(expected);
            //Act
            var actual = await service.GetUserOTPByIdAsync(userId, siteId);

            //Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers

        private UserOtpService GetUserOtpService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new UserOtpService(baseService);
            return service;
        }

        #endregion Helpers
    }
}