namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class ProcessRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int ProcessedBy { get; set; }

        public int RoleId { get; set; }

        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public string Action { get; set; }

        public override string GetSettingKey()
        {
            var key = string.Empty;
            if (Action == RequestCommand.NotAccept.ToString()
                || Action == RequestCommand.Reject.ToString()
                || Action == RequestCommand.Void.ToString())
            {
                key = "WorkingTimeRequest_RejectRequest";
            }
            else if (Action == RequestCommand.Reset.ToString())
            {
                key = "WorkingTimeRequest_ResetRequest";
            }
            else
            {
                key = "WorkingTimeRequest_VoidRequest_bySA";
            }

            return key;
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}