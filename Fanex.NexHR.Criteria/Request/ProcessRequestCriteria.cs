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
        public RequestCommand Command { get; set; }

        public override string GetSettingKey()
        {
            var key = string.Empty;
            if (Command == RequestCommand.NotAccept
                || Command == RequestCommand.Reject
                || Command == RequestCommand.Void)
            {
                key = "WorkingTimeRequest_RejectRequest";
            }
            else if (Command == RequestCommand.Reset)
            {
                key = "WorkingTimeRequest_ResetRequest";
            }
            else
            {
                key = "WorkingTimeRequest_VoidRequest_bySA";
            }

            return key;
        }

        public override bool IsValid() => RequestId > 0;
    }
}