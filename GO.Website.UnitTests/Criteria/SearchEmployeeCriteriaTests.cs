namespace GO.Website.UnitTests.Criteria
{
    using GO.Website.Criteria;
    using Xunit;

    public class SearchEmployeeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new SearchEmployeeCriteria();
            var expected = "HRM_Employee_Search";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("test")]
        public void IsValid_WhenCalled_ReturnTrue(string Keyword)
        {
            // Arrange
            var criteria = new SearchEmployeeCriteria(){
                Keyword = Keyword
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("")]
        public void IsValid_WhenCalled_ReturnFalse(string Keyword)
        {
            // Arrange
            var criteria = new SearchEmployeeCriteria()
            {
                Keyword = Keyword
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
