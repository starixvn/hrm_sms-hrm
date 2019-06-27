namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum CheckInOutReasons
    {
        [StringValue("Van Late")]
        VanLate = 1,

        [StringValue("Business Trip")]
        BusinessTrip = 2,

        [StringValue("Forgot")]
        Forgot = 3,

        [StringValue("Other")]
        Other = 4
    }
}
