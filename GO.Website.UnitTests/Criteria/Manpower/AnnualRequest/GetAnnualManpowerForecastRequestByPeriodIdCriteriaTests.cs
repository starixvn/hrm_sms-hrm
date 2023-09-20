namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetAnnualManpowerForecastRequestByPeriodIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetAnnualManpowerForecastRequestByPeriodIdCriteria();
            var expected = "Manpower_AnnualRequestForm_GetByPeriodId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(short periodId)
        {
            // Arrange
            var criteria = new GetAnnualManpowerForecastRequestByPeriodIdCriteria
            {
                PeriodId = periodId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalledWithParamInValid_ReturnFalse()
        {
            // Arrange
            var criteria = new GetAnnualManpowerForecastRequestByPeriodIdCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}