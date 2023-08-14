namespace GO.Website.UnitTests.Criteria.Manpower.Period
{
    using GO.Website.Criteria.Manpower.Period;
    using Xunit;

    public class CreatePeriodCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new CreatePeriodCommand();
            var expected = "Manpower_Period_Insert";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test", 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(string name, int createdBy)
        {
            // Arrange
            var criteria = new CreatePeriodCommand()
            {
                Name = name,
                CreatedBy = createdBy
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
            var criteria = new CreatePeriodCommand()
            {
                Name = "",
                CreatedBy = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
