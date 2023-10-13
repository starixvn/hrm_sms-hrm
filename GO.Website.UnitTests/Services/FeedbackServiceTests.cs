namespace GO.Website.UnitTests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Services;
    using Extensions;
    using Models.Feedback;
    using Models.Todolist;
    using NSubstitute;
    using Website.Criteria.Feedback;
    using Website.Services;
    using Xunit;

    public class FeedbackServiceTests
    {
        private IBaseService baseService;
        private readonly Fixture fixture = new();

        [Fact]
        public async Task GetTodolistByUserIdAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var userId = fixture.Create<int>();
            var periods = fixture.Create<IEnumerable<Period>>();

            var toDoList = new List<TodolistItem>();
            foreach (var period in periods)
            {
                var item = new TodolistItem
                {
                    Name = string.Format("<div><strong>{0}</strong> is ready for your evaluation on your Superior(s) & Team-mate(s).</div>" +
                                         "<div>You are given a period from <strong>{1}</strong> until <strong>{2}</strong> to complete this evaluation.</div>",
                        period.Name,
                        period.FromDate.ToStartOfDayString(),
                        period.ToDate.ToEndOfDayString()),
                    Actions = new List<TodolistAction>
                    {
                        new TodolistAction
                        {
                            Name = "View",
                            Method = "GET",
                            IsRedirect = true,
                            Url = "/Feedback/Index"
                        }
                    }
                };
                toDoList.Add(item);
            }

            baseService.GetDataAsync<Period>(Arg.Any<GetTodolistFeedbackByUserIdCriteria>()).Returns(periods);
            //Act
            var actual = await service.GetTodolistByUserIdAsync(userId);

            //Assert
            Assert.Equal(toDoList.Count, actual.Count);
            Assert.Equal(toDoList.First().Name, actual.First().Name);
        }
        #region Helpers

        private FeedbackService GetManpowerForecastService()
        {
            baseService = Substitute.For<IBaseService>();
            var service = new FeedbackService(baseService);

            return service;
        }

        #endregion Helpers
    }
}
