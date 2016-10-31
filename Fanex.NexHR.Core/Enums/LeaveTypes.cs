namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum LeaveTypes : int
    {
        [StringValue("General Leave")]
        GeneralLeave = 0,

        [StringValue("Annual Leave")]
        AnnualLeave = 2,

        [StringValue("Unpaid Leave")]
        UnpaidLeave = 25
    }
}