namespace GO.WebAPI.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }

        public override string GetSettingKey() => "API_Function_GetByUserId";

        public override bool IsValid() => UserId > 0 && SiteId > 0;
    }
}