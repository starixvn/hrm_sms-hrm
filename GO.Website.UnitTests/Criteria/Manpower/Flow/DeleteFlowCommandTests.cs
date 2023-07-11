namespace GO.Website.UnitTests.Criteria.Manpower.Flow
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class DeleteFlowCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new DeleteFlowCommand();
            var expected = "Manpower_Flow_Delete";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test", 1)]
        public void IsValid_WhenCalled_ReturnTrue(string unitName, int modifiedBy)
        {
            // Arrange
            var criteria = new DeleteFlowCommand()
            {
                UnitName = unitName,
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
            var criteria = new DeleteFlowCommand()
            {
                UnitName = "",
                ModifiedBy = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
