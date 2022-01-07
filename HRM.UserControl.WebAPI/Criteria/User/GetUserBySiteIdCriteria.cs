namespace HRM.UserControl.WebAPI.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserBySiteIdCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public short SiteId { get; set; }
        public byte? StatusId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "API_User_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}