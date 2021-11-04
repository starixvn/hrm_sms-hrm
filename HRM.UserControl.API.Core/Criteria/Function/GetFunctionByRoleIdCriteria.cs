namespace HRM.UserControl.API.Core.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionByRoleIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }
        public short RoleId { get; set; }

        public override string GetSettingKey() => "API_Function_GetByRoleId";

        public override bool IsValid() => SiteId > 0;
    }
}