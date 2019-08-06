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

        [StringValue("Paternity Leave")]
        PaternityLeave = 14,

        [StringValue("Unpaid Leave")]
        UnpaidLeave = 25,

        [StringValue("Unpaid Leave (Insurance)")]
        UnpaidLeaveInsurance = 41
    }
}