namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class UpdateStatusPeriodCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusPeriodCommand();
            var expected = "Manpower_Period_UpdateStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(short periodId, byte isActive, int modifiedBy)
        {
            // Arrange
            var criteria = new UpdateStatusPeriodCommand()
            {
                PeriodId = periodId,
                IsActive = isActive,
                ModifiedBy = modifiedBy
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
            var criteria = new UpdateStatusPeriodCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
