namespace HRM.UserControl.Admin.Website.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "Function_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }

}