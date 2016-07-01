namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class DeleteShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey()
        {
            return "Shift_Delete";
        }

        public override bool IsValid()
        {
            return ShiftId > 0;
        }
    }
}
