namespace GO.Website.UnitTests.Criteria.Account
{
    using Website.Criteria.Account;
    using Xunit;

    public class UpdateStatusUserOTPCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusUserOTPCommand();
            var expected = "UserOTP_UpdateStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(int userId, short siteId)
        {
            // Arrange
            var criteria = new UpdateStatusUserOTPCommand()
            {
                UserId = userId,
                SiteId = siteId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0, 0)]
        public void IsValid_WhenCalledWithParamInValid_ReturnFalse(int userId, short siteId)
        {
            // Arrange
            var criteria = new UpdateStatusUserOTPCommand()
            {
                UserId = userId,
                SiteId = siteId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
