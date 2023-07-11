namespace GO.Website.UnitTests.Criteria.Manpower
{
    using AutoFixture;
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class GetEmployeeCriteriaTests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetEmployeeCriteria();
            var expected = "Manpower_Employee_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = fixture.Create<GetEmployeeCriteria>();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

    }
}
