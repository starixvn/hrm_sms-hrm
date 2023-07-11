namespace GO.Website.UnitTests.Criteria.Manpower
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetUnitByUserIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUnitByUserIdCriteria();
            var expected = "Manpower_Unit_GetByUserId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalled_ReturnTrue(int userId)
        {
            // Arrange
            var criteria = new GetUnitByUserIdCriteria()
            {
                UserId = userId
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
            var criteria = new GetUnitByUserIdCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
