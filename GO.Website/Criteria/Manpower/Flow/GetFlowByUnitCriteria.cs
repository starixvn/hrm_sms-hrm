namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetFlowByUnitCriteria : CriteriaBase
    {
        public string UnitName { get; set; }

        public override string GetSettingKey() => "Manpower_Flow_GetByUnit";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(UnitName);
    }
}