namespace GO.Website.UnitTests.Criteria.Manpower
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class UpdateExcludedEmployeeCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateExcludedEmployeeCommand();
            var expected = "Manpower_ExcludedEmployee_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1,true, 1)]
        public void IsValid_WhenCalled_ReturnTrue(short siteId, int empId, bool isExcluded , int modifiedBy)
        {
            // Arrange
            var criteria = new UpdateExcludedEmployeeCommand()
            {
                SiteId = siteId,
                EmpId = empId,
                IsExcluded = isExcluded,
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
            var criteria = new UpdateExcludedEmployeeCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
