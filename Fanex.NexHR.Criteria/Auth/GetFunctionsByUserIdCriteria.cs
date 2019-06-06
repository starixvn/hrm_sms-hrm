namespace Fanex.NexHR.Criteria.Auth
{
    using Fanex.Data.Repository;

    public class GetFunctionByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override bool IsValid() => UserId > 0;

        public override string GetSettingKey() => "Function_GetFunctionsByUserId";
    }
}
