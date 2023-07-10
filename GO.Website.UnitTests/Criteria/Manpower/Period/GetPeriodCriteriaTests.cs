namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class GetPeriodCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetPeriodCriteria();
            var expected = "Manpower_Period_Get";

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
            var criteria = new GetPeriodCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
