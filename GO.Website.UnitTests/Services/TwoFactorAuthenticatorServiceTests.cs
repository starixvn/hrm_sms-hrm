namespace GO.Website.UnitTests.Services
{
    using System;
    using AutoFixture;
    using GO.Website.Models;
    using GO.Website.Services;
    using Google.Authenticator;
    using Xunit;

    public class TwoFactorAuthenticatorServiceTests
    {
        private OtpConfig otpConfig;
        private readonly Fixture fixture = new();

        [Fact]
        public void GenerateSetupCode_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetTwoFactorAuthenticatorService();

            var email = fixture.Create<string>();
            var secretKey = fixture.Create<string>();
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var expected = twoFactorAuthen.GenerateSetupCode(otpConfig.AppName, email, secretKey, false);

            //Act
            var actual = service.GenerateSetupCode(email, secretKey);

            //Assert
            Assert.Equal(expected.ManualEntryKey, actual);
        }

        [Fact]
        public void ValidateOTPCode_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetTwoFactorAuthenticatorService();
            var otpCode = fixture.Create<string>();
            var secretKey = fixture.Create<string>();

            var timeExpireOTP = TimeSpan.FromSeconds(otpConfig.OtpExpireTime);
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);

            //Act
            var actual = service.ValidateOTPCode(secretKey, otpCode);

            //Assert
            Assert.Equal(validOTP, actual);
        }

        #region Helpers

        private TwoFactorAuthenticatorService GetTwoFactorAuthenticatorService()
        {
            otpConfig = fixture.Create<OtpConfig>();
            var service = new TwoFactorAuthenticatorService(otpConfig);
            return service;
        }

        #endregion Helpers
    }
}