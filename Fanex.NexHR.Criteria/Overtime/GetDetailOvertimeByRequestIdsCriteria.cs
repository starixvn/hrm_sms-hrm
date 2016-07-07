namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    public class GetDetailOvertimeByRequestIdsCriteria : CriteriaBase
    {
        public string ListRequestID { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetDetailByListRequestID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}