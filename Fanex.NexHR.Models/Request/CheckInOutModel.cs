namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class CheckInOutModel : BaseRequest
    {
        public DateTime OnDate { get; set; }

        public DateTime RequestTime { get; set; }

        public byte ReasonId { get; set; }

        public string ShiftName { get; set; }

        public string ReasonName
        {
            get
            {
                switch (ReasonId)
                {
                    case (byte)CheckInOutReasons.VanLate:
                        return Labels.VanLate;
                    case (byte)CheckInOutReasons.BusinessTrip:
                        return Labels.BusinessTrip;
                    case (byte)CheckInOutReasons.Forgot:
                        return Labels.Forgot;
                    case (byte)CheckInOutReasons.Other:
                        return Labels.Other;
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
