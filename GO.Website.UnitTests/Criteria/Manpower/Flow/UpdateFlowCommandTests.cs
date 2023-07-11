namespace GO.Website.UnitTests.Criteria.Manpower.Flow
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class UpdateFlowCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateFlowCommand();
            var expected = "Manpower_Flow_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test","tests", 1)]
        public void IsValid_WhenCalled_ReturnTrue(string unitName, string flowXml , int modifiedBy)
        {
            // Arrange
            var criteria = new UpdateFlowCommand()
            {
                UnitName = unitName,
                FlowXml = flowXml,
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
            var criteria = new UpdateFlowCommand()
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
