namespace GO.Website.UnitTests.Criteria.Report.Manpower
{
    using Website.Criteria.Report.Manpower;
    using Xunit;

    public class GetManpowerForecastReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetManpowerForecastReportCriteria();
            var expected = "Report_ManpowerForecast_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 1, "")]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(int userId, short siteId, short periodId, string unitType)
        {
            // Arrange
            var criteria = new GetManpowerForecastReportCriteria()
            {
                UserId = userId,
                SiteId = siteId,
                PeriodId = periodId,
                UnitType = unitType
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        public void IsValid_WhenCalledWithParamInValid_ReturnFalse(int userId, short siteId, short periodId)
        {
            // Arrange
            var criteria = new GetManpowerForecastReportCriteria()
            {
                UserId = userId,
                SiteId = siteId,
                PeriodId = periodId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
