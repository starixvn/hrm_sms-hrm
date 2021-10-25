namespace HRM.UserControl.Admin.Website.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetAllUserCriteria : CriteriaBase
    {
        public string Username { get; set; }
        public short? SiteId { get; set; }
        public byte? StatusId { get; set; }
        public int PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "User_GetAll";

        public override bool IsValid() => true;
    }
}