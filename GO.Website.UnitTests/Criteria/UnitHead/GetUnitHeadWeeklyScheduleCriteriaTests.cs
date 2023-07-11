namespace GO.Website.UnitTests.Criteria.UnitHead
{
    using AutoFixture;
    using GO.Website.Criteria.UnitHead;
    using System;
    using Xunit;

    public class GetUnitHeadWeeklyScheduleCriteriaTests
    {
        private readonly Fixture fixture = new();

        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUnitHeadWeeklyScheduleCriteria();
            var expected = "UnitHead_GetSchedule";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUnitHeadWeeklyScheduleCriteria() {
                CompanyId = fixture.Create<short>(),
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
