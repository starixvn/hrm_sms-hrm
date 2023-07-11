namespace GO.Website.UnitTests.Criteria.Feedback
{
    using GO.Website.Criteria.Feedback;
    using Xunit;

    public class GetTodolistFeedbackByUserIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetTodolistFeedbackByUserIdCriteria();
            var expected = "GO_GetTodolistByUserId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        public void IsValid_WhenCalled_ReturnTrue(int userId)
        {
            // Arrange
            var criteria = new GetTodolistFeedbackByUserIdCriteria() {
                UserId = userId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
