namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetWorkingShiftLastSyncDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncData_GetWorkingShiftLastSyncDate";

        public override bool IsValid() => true;
    }
}
