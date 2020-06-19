namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetUnpaidLeaveTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Report_GetUnpaidLeaveType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
