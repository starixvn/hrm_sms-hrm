namespace Fanex.NexHR.Criteria.ManagementLevel
{
    using Fanex.Data.Repository;

    public class GetManagementLevelCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "ManagementLevel_Get";

        public override bool IsValid() => true;
    }
}
