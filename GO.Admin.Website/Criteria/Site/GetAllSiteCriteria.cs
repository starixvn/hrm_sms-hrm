namespace GO.Admin.Website.Criteria.Site
{
    using Fanex.Data.Repository;

    public class GetAllSiteCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Site_GetAll";

        public override bool IsValid() => true;
    }
}