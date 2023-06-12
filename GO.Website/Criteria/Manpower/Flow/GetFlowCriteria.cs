namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetFlowCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Manpower_Flow_Get";

        public override bool IsValid() => true;
    }
}