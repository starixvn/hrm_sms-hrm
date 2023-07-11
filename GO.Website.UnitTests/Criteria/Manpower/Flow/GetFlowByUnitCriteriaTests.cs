namespace GO.Website.UnitTests.Criteria.Manpower.Flow
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetFlowByUnitCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetFlowByUnitCriteria();
            var expected = "Manpower_Flow_GetByUnit";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test")]
        public void IsValid_WhenCalled_ReturnTrue(string unitName)
        {
            // Arrange
            var criteria = new GetFlowByUnitCriteria()
            {
                UnitName = unitName
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
            var criteria = new GetFlowByUnitCriteria()
            {
                UnitName = ""
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
