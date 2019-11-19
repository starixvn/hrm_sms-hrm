namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Ok.")]
    public enum LeaveTypes
    {
        [StringValue("General Leave")]
        GeneralLeave = 0,

        [StringValue("Annual Leave")]
        AnnualLeave = 2,

        [StringValue("Employee Wedding (3 Days)")]
        WeddingLeave = 6,

        [StringValue("Business Trip")]
        BusinessTrip = 9,

        [StringValue("Keep in view (by Day)")]
        KeepInViewByDay = 10,

        [StringValue("Maternity Leave")]
        MaternityLeave = 12,

        [StringValue("Paternity Leave")]
        PaternityLeave = 14,

        [StringValue("Bereavement Leave")]
        BereavementLeave = 21,

        [StringValue("Unpaid Leave")]
        UnpaidLeave = 25,

        [StringValue("Business during working")]
        BusinessDuringWorking = 26,

        [StringValue("Compensation Leave")]
        CompensationLeave = 27,

        [StringValue("Unpaid Leave (Insurance)")]
        UnpaidLeaveInsurance = 41,

        [StringValue("Keep in view (by Hour)")]
        KeepInViewByHour = 48
    }
}