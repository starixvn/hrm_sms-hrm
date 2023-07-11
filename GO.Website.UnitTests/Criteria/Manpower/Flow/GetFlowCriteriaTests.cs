namespace GO.Website.UnitTests.Criteria.Manpower.Flow
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetFlowCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetFlowCriteria();
            var expected = "Manpower_Flow_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetFlowCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
