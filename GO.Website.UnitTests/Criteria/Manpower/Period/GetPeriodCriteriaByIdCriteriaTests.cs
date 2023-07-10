namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class GetPeriodCriteriaByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetPeriodCriteriaByIdCriteria();
            var expected = "Manpower_Period_GetById";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalled_ReturnTrue(short periodId)
        {
            // Arrange
            var criteria = new GetPeriodCriteriaByIdCriteria()
            {
                PeriodId = periodId
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
            var criteria = new GetPeriodCriteriaByIdCriteria()
            {
                PeriodId = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
