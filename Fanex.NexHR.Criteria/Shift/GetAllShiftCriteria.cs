namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class GetAllShiftCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Shift_GetShifts";

        public override bool IsValid() => true;
    }
}