namespace GO.Website.UnitTests.Criteria.UnitHead
{
    using AutoFixture;
    using GO.Website.Criteria.UnitHead;
    using System;
    using Xunit;

    public class GetUnitHeadScheduleCriteriaTests
    {
        private readonly Fixture fixture = new();

        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUnitHeadScheduleCriteria();
            var expected = "API_UnitHead_GetSchedule";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUnitHeadScheduleCriteria() {
                FromDate = fixture.Create<DateTime>(),
                ToDate = fixture.Create<DateTime>()
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
