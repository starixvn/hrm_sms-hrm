namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class BaseRequest
    {
        public long RequestId { get; set; }

        public Employee Employee { get; set; }

        public int RequestTypeId { get; set; }

        public string Remark { get; set; }

        public Employee VerifiedBy { get; set; }

        public Employee ApprovedBy { get; set; }

        public Employee ProcessedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime VerifiedDate { get; set; }

        public DateTime ApprovedDate { get; set; }

        public DateTime ProcessedDate { get; set; }

        public DateTime InsertedDate { get; set; }

        public byte CurrentStatus { get; set; }

        public int VerifiedStatus { get; set; }

        public int ApprovedStatus { get; set; }

        public int ProcessedStatus { get; set; }

        public RequestStatus Status
        {
            get
            {
                return new RequestStatus
                {
                    Id = CurrentStatus
                };
            }
        }

        public string RefNO { get; set; }

        public string RequestTypeName
        {
            get
            {
                switch (RequestTypeId)
                {
                    case (int)RequestType.CheckIn:
                        return Labels.CheckIn;
                    case (int)RequestType.CheckOut:
                        return Labels.CheckOut;
                    case (byte)RequestType.LateIn:
                        return Labels.LateIn;
                    case (byte)RequestType.EarlyOut:
                        return Labels.EarlyOut;
                    case (byte)RequestType.ChangeShift:
                        return Labels.ChangeShift;
                    case (byte)RequestType.Leave:
                        return Labels.Leave;
                    case (byte)RequestType.KeepInView:
                        return Labels.KeepInView;
                    case (byte)RequestType.Overtime:
                        return Labels.Overtime;
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
