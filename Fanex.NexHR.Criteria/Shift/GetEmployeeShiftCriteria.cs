namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class GetEmployeeShiftCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeShift_Get";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
