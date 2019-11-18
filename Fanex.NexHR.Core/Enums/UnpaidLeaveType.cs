namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum UnpaidLeaveType
    {
        [StringValue("Unpaid Leave")]
        UnpaidLeave = 1,

        [StringValue("Unpaid Leave (Insurance)")]
        UnpaidLeaveInsurance = 2,

        [StringValue("Paternity Leave")]
        Paternity = 3,

        [StringValue("Maternity Leave")]
        Maternity = 4
    }
}