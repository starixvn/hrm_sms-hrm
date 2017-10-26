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

        [StringValue("Unpaid Leave")]
        UnpaidLeave = 25
    }
}