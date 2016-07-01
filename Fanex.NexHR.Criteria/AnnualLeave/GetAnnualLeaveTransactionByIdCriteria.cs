namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class GetAnnualLeaveTransactionByIdCriteria : CriteriaBase
    {
        public int LeaveTransId { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetByID";
        }

        public override bool IsValid()
        {
            return LeaveTransId > 0;
        }
    }
}
