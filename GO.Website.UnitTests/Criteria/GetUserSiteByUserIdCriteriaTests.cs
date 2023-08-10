namespace GO.Website.UnitTests.Criteria
{
    using GO.Website.Criteria;
    using Xunit;

    public class GetUserSiteByUserIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUserSiteByUserIdCriteria();
            var expected = "GO_User_GetUserSiteByUserId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalled_ReturnTrue(int userId)
        {
            // Arrange
            var criteria = new GetUserSiteByUserIdCriteria() { 
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnFalse()
        {
            // Arrange
            var criteria = new GetUserSiteByUserIdCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
