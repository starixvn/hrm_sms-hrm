namespace GO.Website.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using AutoFixture;
    using GO.Website.Controllers;
    using GO.Website.Models;
    using GO.Website.Services;
    using HRM.UserControl.Client.AspNetCore.Session;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Models.Account;
    using NSubstitute;
    using Website.Criteria.Account;
    using Xunit;

    public class AccountControllerTests
    {
        private IUserControlSettings userControlSettings;
        private IUserControlSession userControlSession;
        private IUserOtpService userOtpService;
        private OtpConfig otpConfig;
        private ITwoFactorAuthenticatorService twoFactorAuthService;
        private IUserService userService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Setup2Fa_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.Setup2Fa();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Account/Setup2FA.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task VerifyOTP_WhenCalledWithOtpConfigTrue_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var userOtp = new UserOtp {
                Enabled = true
            };
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            otpConfig.Enable = true;

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            //Act
            var result = await controller.VerifyOTP();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Account/VerifyOTP.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetAccountOTP_WhenCalledUserOtpNull_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var user = fixture.Create<User>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns((UserOtp)null);
            userService.GetByIdAsync(userControlSession.UserId).Returns(user);
            //Act
            var result = await controller.GetAccountOTP();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Null(jsonResult.Value);
        }

        [Fact]
        public async Task GetAccountOTP_WhenCalledSecretKeyNull_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            userOtp.SecretKey = null;
            var user = fixture.Create<User>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            userService.GetByIdAsync(userControlSession.UserId).Returns(user);
            //Act
            var result = await controller.GetAccountOTP();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(userOtp, jsonResult.Value);
        }

        [Fact]
        public async Task GetAccountOTP_WhenCalledSecretKeyNotNull_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var user = fixture.Create<User>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            userService.GetByIdAsync(userControlSession.UserId).Returns(user);
            //Act
            var result = await controller.GetAccountOTP();
            if (!string.IsNullOrWhiteSpace(userOtp.SecretKey))
            {
                userOtp.RecoveryCode = twoFactorAuthService.GenerateSetupCode(user.Email, userOtp.SecretKey);
            }
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(userOtp, jsonResult.Value);
        }

        [Fact]
        public async Task SetupOtp_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            var user = fixture.Create<User>();

            userService.GetByIdAsync(userControlSession.UserId).Returns(user);
            //Act
            var result = await controller.SetupOtp();
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var appName = jsonResult.Value.GetType().GetProperty("AppName")?.GetValue(jsonResult.Value);
            var email = jsonResult.Value.GetType().GetProperty("Email")?.GetValue(jsonResult.Value);
            Assert.Equal(email, user.Email);
            Assert.Equal(appName, otpConfig.AppName);
        }

        [Fact]
        public async Task VerifySetupOTP_WhenCalledOtpTrue_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var model = fixture.Create<SetupOTP>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            twoFactorAuthService.ValidateOTPCode(userOtp.SecretKey, model.Otp).Returns(true);
            userOtpService.UpdateOTPStatusAsync(Arg.Any<UpdateStatusUserOTPCommand>()).Returns(true);

            //Act
            var result = await controller.VerifySetupOTP(model);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(true, jsonResult.Value);
        }

        [Fact]
        public async Task VerifySetupOTP_WhenCalledOtpFalse_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var model = fixture.Create<SetupOTP>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            twoFactorAuthService.ValidateOTPCode(userOtp.SecretKey, model.Otp).Returns(false);
            userOtpService.UpdateOTPStatusAsync(Arg.Any<UpdateStatusUserOTPCommand>()).Returns(false);

            //Act
            var result = await controller.VerifySetupOTP(model);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(false, jsonResult.Value);
        }

        [Fact]
        public async Task ResetSetupOtp_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var isSuccessful = fixture.Create<bool>();

            userOtpService.UpdateOTPStatusAsync(Arg.Any<UpdateStatusUserOTPCommand>()).Returns(isSuccessful);

            //Act
            var result = await controller.ResetSetupOtp(userOtp);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(isSuccessful, jsonResult.Value);
        }

        [Fact]
        public async Task UpdateStatusOTP_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var isSuccessful = fixture.Create<bool>();

            userOtpService.UpdateOTPStatusAsync(Arg.Any<UpdateStatusUserOTPCommand>()).Returns(isSuccessful);

            //Act
            var result = await controller.UpdateStatusOTP(userOtp);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(isSuccessful, jsonResult.Value);
        }

        [Fact]
        public async Task VerifyOTP_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            userControlSession.SiteId = fixture.Create<short>();
            var userOtp = fixture.Create<UserOtp>();
            var model = fixture.Create<SetupOTP>();
            var isOTPValid = fixture.Create<bool>();

            userOtpService.GetUserOTPByIdAsync(userControlSession.UserId, userControlSession.SiteId).Returns(userOtp);
            twoFactorAuthService.ValidateOTPCode(userOtp.SecretKey, model.Otp).Returns(isOTPValid);

            //Act
            var result = await controller.VerifyOTP(model);
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(isOTPValid, jsonResult.Value);
        }
        #region helper

        private AccountController GetController()
        {
            userControlSettings = Substitute.For<IUserControlSettings>();
            userControlSession = Substitute.For<IUserControlSession>();
            userOtpService = Substitute.For<IUserOtpService>();
            otpConfig = fixture.Create<OtpConfig>();
            twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            userService = Substitute.For<IUserService>();

            return new AccountController(
                userControlSettings,
                userControlSession,
                twoFactorAuthService,
                userOtpService,
                userService,
                otpConfig
            );
        }

        #endregion helper
    }
}
