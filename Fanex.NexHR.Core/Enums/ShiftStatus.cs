namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum ShiftStatus : byte
    {
        [StringValue("All")]
        All = 0,

        [StringValue("Active")]
        Active = 1,

        [StringValue("Disabled")]
        Disabled = 2
    }
}