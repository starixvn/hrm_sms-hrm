namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using Xunit;

    public class UpdateAnnualManpowerForecastRequestCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateAnnualManpowerForecastRequestCommand();
            var expected = "Manpower_AnnualRequestForm_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, "", 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(short requestId, int modifiedId, string detailXml, byte statusId)
        {
            // Arrange
            var criteria = new UpdateAnnualManpowerForecastRequestCommand()
            {
                RequestId = requestId,
                ModifiedBy = modifiedId,
                DetailXml = detailXml,
                StatusId = statusId
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
            var criteria = new UpdateAnnualManpowerForecastRequestCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
