namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetCompanyCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_Company_Get";

        public override bool IsValid() => true;
    }
}