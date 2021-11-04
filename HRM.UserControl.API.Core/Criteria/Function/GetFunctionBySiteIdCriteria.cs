namespace HRM.UserControl.API.Core.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "API_Function_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}