namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using Xunit;

    public class UpdateStatusMultiAnnualManpowerForecastRequestCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusMultiAnnualManpowerForecastRequestCommand();
            var expected = "Manpower_AnnualRequestForm_UpdateStatusMultiRequest";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1", 1)]
        public void IsValid_WhenCalled_ReturnTrue(string requestIds, int modifiedId)
        {
            // Arrange
            var criteria = new UpdateStatusMultiAnnualManpowerForecastRequestCommand
            {
                RequestIds = requestIds,
                ModifiedBy = modifiedId
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
            var criteria = new UpdateStatusMultiAnnualManpowerForecastRequestCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}