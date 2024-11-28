namespace Fanex.NexHR.Criteria.SystemSetting
{
    using Fanex.Data.Repository;

    public class GetEnableGroupDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Configure_GetEnableGroupDate";

        public override bool IsValid() => true;
    }
}