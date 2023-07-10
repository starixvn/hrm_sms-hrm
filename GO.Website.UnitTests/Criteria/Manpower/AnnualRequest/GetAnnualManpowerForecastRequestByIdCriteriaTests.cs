namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetAnnualManpowerForecastRequestByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria();
            var expected = "Manpower_AnnualRequestForm_GetByRequestId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalled_ReturnTrue(short requestId)
        {
            // Arrange
            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria()
            {
                RequestId = requestId
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
            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
