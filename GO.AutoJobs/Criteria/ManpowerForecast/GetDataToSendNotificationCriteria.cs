namespace GO.AutoJobs.Criteria.ManpowerForecast
{
    using Fanex.Data.Repository;

    internal class GetDataToSendNotificationCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "AutoJob_ManpowerForecast_GetDataToSendNotification";

        public override bool IsValid() => true;
    }
}