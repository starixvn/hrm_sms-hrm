namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Criteria.Feedback;
    using GO.Website.Extensions;
    using GO.Website.Models.Feedback;
    using GO.Website.Models.Todolist;

    public class FeedbackService : IFeedbackService
    {
        private readonly IBaseService baseService;

        public FeedbackService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IList<TodolistItem>> GetTodolistByUserIdAsync(int userId)
        {
            var criteria = new GetTodolistFeedbackByUserIdCriteria
            {
                UserId = userId
            };
            var periods = await baseService.GetDataAsync<Period>(criteria);
            var items = new List<TodolistItem>();
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
                items.Add(item);
            }

            return items;
        }
    }
}