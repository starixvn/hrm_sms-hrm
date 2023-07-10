namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class DeletePeriodCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new DeletePeriodCommand();
            var expected = "Manpower_Period_Delete";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsValid_WhenCalled_ReturnTrue(short periodId, int modifiedBy)
        {
            // Arrange
            var criteria = new DeletePeriodCommand()
            {
                PeriodId = periodId,
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
            var criteria = new DeletePeriodCommand()
            {
                PeriodId = 0,
                ModifiedBy = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
