namespace GO.Admin.Website.Criteria.User
{
    using Fanex.Data.Repository;

    public class SearchUserMappingCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public short? SiteId { get; set; }
        public byte? StatusId { get; set; }

        public override string GetSettingKey() => "UserMapping_Search";

        public override bool IsValid() => true;
    }
}