namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class VoidRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public int RoleId { get; set; }
        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public RequestType RequestTypeId { get; set; }

        public override string GetSettingKey()
        {
            var key = string.Empty;
            switch (RequestTypeId)
            {
                case RequestType.CheckIn:
                case RequestType.CheckOut:
                    key = "CheckInOutRequest_Void";
                    break;

                case RequestType.LateIn:
                case RequestType.EarlyOut:
                    key = "LateInEarlyOutRequest_Void";
                    break;

                case RequestType.ChangeShift:
                    key = "ChangeShiftRequest_Void";
                    break;

                case RequestType.Leave:
                    key = "LeaveRequest_Void";
                    break;

                default:
                    key = "KeepInViewRequest_Void";
                    break;
            }

            return key;
        }

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}