namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetDataForCreateAnnualManpowerForecastRequestCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria();
            var expected = "Manpower_AnnualRequestForm_GetDataForCreate";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "test")]
        public void IsValid_WhenCalled_ReturnTrue(int userId, string unitName)
        {
            // Arrange
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria()
            {
                UnitName = unitName,
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
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
