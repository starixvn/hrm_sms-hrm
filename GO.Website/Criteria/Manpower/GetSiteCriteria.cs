namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetSiteCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_Site_Get";

        public override bool IsValid() => true;
    }
}