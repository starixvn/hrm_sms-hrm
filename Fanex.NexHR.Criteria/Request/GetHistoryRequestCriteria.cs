namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    public class GetHistoryRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
