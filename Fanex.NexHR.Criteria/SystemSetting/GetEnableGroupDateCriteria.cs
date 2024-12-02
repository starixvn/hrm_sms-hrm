namespace Fanex.NexHR.Criteria.SystemSetting
{
    using Fanex.Data.Repository;

    public class GetEnableGroupDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SystemConfigs_GetEnableGroupDate";

        public override bool IsValid() => true;
    }
}