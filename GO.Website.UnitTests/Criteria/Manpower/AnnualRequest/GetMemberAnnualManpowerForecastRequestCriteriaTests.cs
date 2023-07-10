namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetMemberAnnualManpowerForecastRequestCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMemberAnnualManpowerForecastRequestCriteria();
            var expected = "Manpower_AnnualRequestForm_GetMemberRequest";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsValid_WhenCalled_ReturnTrue(short periodId, int userId)
        {
            // Arrange
            var criteria = new GetMemberAnnualManpowerForecastRequestCriteria()
            {
                PeriodId = periodId,
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
            var criteria = new GetMemberAnnualManpowerForecastRequestCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
