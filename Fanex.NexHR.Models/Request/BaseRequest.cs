namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class BaseRequest
    {
        public long RequestId { get; set; }

        public Employee Employee { get; set; }

        public byte RequestTypeId { get; set; }

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

        public RequestStatus Status => new RequestStatus
        {
            Id = CurrentStatus
        };

        public string RefNO { get; set; }

        public string RequestTypeName
        {
            get
            {
                var requestTypeName = string.Empty;
                switch (RequestTypeId)
                {
                    case (byte)RequestType.CheckIn:
                        requestTypeName = Labels.CheckIn;
                        break;

                    case (byte)RequestType.CheckOut:
                        requestTypeName = Labels.CheckOut;
                        break;

                    case (byte)RequestType.LateIn:
                        requestTypeName = Labels.LateIn;
                        break;

                    case (byte)RequestType.EarlyOut:
                        requestTypeName = Labels.EarlyOut;
                        break;

                    case (byte)RequestType.ChangeShift:
                        requestTypeName = Labels.ChangeShift;
                        break;

                    case (byte)RequestType.Leave:
                        requestTypeName = Labels.Leave;
                        break;

                    case (byte)RequestType.KeepInView:
                        requestTypeName = Labels.KeepInView;
                        break;

                    case (byte)RequestType.Overtime:
                        requestTypeName = Labels.Overtime;
                        break;

                    default:
                        requestTypeName = string.Empty;
                        break;
                }

                return requestTypeName;
            }
        }
    }
}