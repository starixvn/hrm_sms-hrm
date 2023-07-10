namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetMyAnnualManpowerForecastRequestCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria();
            var expected = "Manpower_AnnualRequestForm_GetMyRequest";

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
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria()
            {
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
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
