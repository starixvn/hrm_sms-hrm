namespace Fanex.NexHR.Models
{
    public class TotalWorkingHours
    {
        public Employee Employee { get; set; }

        public int WorkingDay { get; set; }

        public float ActualWorkingDay { get; set; }

        public float WorkingHours { get; set; }

        public float PaymentWorkingDay { get; set; }

        public float AnnualLeave { get; set; }

        public float BusinessTrip { get; set; }

        public float BereavementLeave { get; set; }

        public float Compensationleave { get; set; }

        public float WeddingLeave { get; set; }

        public float UnpaidLeave { get; set; }

        public float UnpaidLeaveInsurance { get; set; }

        public float PeternityLeave { get; set; }

        public float MaternityLeave { get; set; }

        public float TotalUnpaidLeaveInsurance
        {
            get
            {
                return UnpaidLeaveInsurance + PeternityLeave + MaternityLeave;
            }
        }

        public float TotalPaymentWorkingDay
        {
            get
            {
                return PaymentWorkingDay + AnnualLeave + BusinessTrip + BereavementLeave + Compensationleave + WeddingLeave;
            }
        }

        public float TotalHours { get; set; }

        public float OTNormalHours { get; set; }

        public float OTOffHours { get; set; }

        public float OTPublicHours { get; set; }
    }
}