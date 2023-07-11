namespace GO.Website.UnitTests.Criteria.UnitHead
{
    using GO.Website.Criteria.UnitHead;
    using Xunit;

    public class GetUnitHeadInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUnitHeadInfoCriteria();
            var expected = "API_UnitHead_GetInfo";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUnitHeadInfoCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
