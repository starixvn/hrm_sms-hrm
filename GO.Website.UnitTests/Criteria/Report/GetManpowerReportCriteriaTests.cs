namespace GO.Website.UnitTests.Criteria.Report
{
    using GO.Website.Criteria.Report;
    using Xunit;

    public class GetManpowerReportCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetJobLevelSummaryReportCriteria();
            var expected = "Report_JobLevelSummary_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobLevelSummaryReportCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
