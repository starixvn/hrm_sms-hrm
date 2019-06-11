namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class KeepInViewModel
    {
        public long RequestId { get; set; }

        public byte KeepInViewTypeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime OnDate { get; set; }

        public bool IsHalfDay { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

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

        public string KeepInViewTypeName
        {
            get
            {
                return KeepInViewTypeId == (int)KeepInViewTypes.KeepInViewByDay
                    ? Labels.ByDay 
                    : Labels.ByHour;
            }
        }
    }
}
