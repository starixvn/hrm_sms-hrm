namespace Fanex.NexHR.Models.Request
{
    using System;

    public class BaseRequest
    {
        public long RequestId { get; set; }

        public Employee Employee { get; set; }

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
    }
}
