namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetUnitCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_Flow_GetUnit";

        public override bool IsValid() => true;
    }
}