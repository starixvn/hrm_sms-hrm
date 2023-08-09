namespace GO.Website.Criteria.RoleAssignment
{
    using Fanex.Data.Repository;

    public class GetUserRoleCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public short SiteId { get; set; }
        public byte? StatusId { get; set; }
        public int PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "GO_UserRole_Get";

        public override bool IsValid() => SiteId > 0;
    }
}