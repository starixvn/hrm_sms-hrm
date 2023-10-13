namespace GO.Website.UnitTests.Criteria.Report
{
    using Website.Criteria.Report;
    using Xunit;

    public class GetUnitCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUnitCriteria();
            var expected = "Manpower_GetUnit";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(int userId, short siteId)
        {
            // Arrange
            var criteria = new GetUnitCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
