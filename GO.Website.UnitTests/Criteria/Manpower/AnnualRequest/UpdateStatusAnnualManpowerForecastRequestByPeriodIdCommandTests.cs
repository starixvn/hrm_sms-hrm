namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using Xunit;

    public class UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommand();
            var expected = "Manpower_AnnualRequestForm_UpdateStatusByPeriodId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, "", 1)]
        public void IsValid_WhenCalled_ReturnTrue(short periodId, int modifiedId, string unitType, byte statusId)
        {
            // Arrange
            var criteria = new UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommand()
            {
                PeriodId = periodId,
                ModifiedBy = modifiedId,
                UnitType = unitType,
                StatusId = statusId
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
            var criteria = new UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
