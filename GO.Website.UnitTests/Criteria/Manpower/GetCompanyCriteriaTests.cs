namespace GO.Website.UnitTests.Criteria.Manpower
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetCompanyCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetCompanyCriteria();
            var expected = "Manpower_Company_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetCompanyCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
