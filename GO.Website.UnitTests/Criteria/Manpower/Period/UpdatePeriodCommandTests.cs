namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class UpdatePeriodCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdatePeriodCommand();
            var expected = "Manpower_Period_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "test", 1)]
        public void IsValid_WhenCalled_ReturnTrue(short periodId, string name, int modifiedBy)
        {
            // Arrange
            var criteria = new UpdatePeriodCommand()
            {
                PeriodId = periodId,
                Name = name,
                ModifiedBy = modifiedBy
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
            var criteria = new UpdatePeriodCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
