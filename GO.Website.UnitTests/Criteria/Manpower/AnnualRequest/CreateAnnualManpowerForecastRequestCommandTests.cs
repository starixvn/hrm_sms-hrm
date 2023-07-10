namespace GO.Website.UnitTests.Criteria.Manpower.AnnualRequest
{
    using GO.Website.Criteria.Manpower;
    using Xunit;

    public class CreateAnnualManpowerForecastRequestCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new CreateAnnualManpowerForecastRequestCommand();
            var expected = "Manpower_AnnualRequestForm_Create";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, "test", 1, "")]
        public void IsValid_WhenCalled_ReturnTrue(short periodId, string unitName, int createdBy, string detailXml)
        {
            // Arrange
            var criteria = new CreateAnnualManpowerForecastRequestCommand()
            {
                PeriodId = periodId,
                UnitName = unitName,
                CreatedBy = createdBy,
                DetailXml = detailXml
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
            var criteria = new CreateAnnualManpowerForecastRequestCommand();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
