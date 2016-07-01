namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class GetShiftByIdCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_GetById";
        }

        public override bool IsValid()
        {
            return ShiftId >= 0;
        }
    }
}
