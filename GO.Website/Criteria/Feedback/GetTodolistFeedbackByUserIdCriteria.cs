namespace GO.Website.Criteria.Feedback
{
    using Fanex.Data.Repository;

    public class GetTodolistFeedbackByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "GO_GetTodolistByUserId";

        public override bool IsValid() => true;
    }
}