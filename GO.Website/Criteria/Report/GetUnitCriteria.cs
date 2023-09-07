namespace GO.Website.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetUnitCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_GetUnit";

        public override bool IsValid() => true;
    }
}
