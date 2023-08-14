namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class UpdateStatusAnnualManpowerForecastRequestCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusAnnualManpowerForecastRequestCommand();
            var expected = "Manpower_AnnualRequestForm_UpdateStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1,1)]
        public void IsValid_WhenCalledWithParamInValid_ReturnTrue(short requestId, int modifiedId)
        {
            // Arrange
            var criteria = new UpdateStatusAnnualManpowerForecastRequestCommand()
            {
                RequestId = requestId,
                ModifiedBy = modifiedId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalledWithParamInValid_ReturnFalse()
        {
            // Arrange
            var criteria = new UpdateStatusAnnualManpowerForecastRequestCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
