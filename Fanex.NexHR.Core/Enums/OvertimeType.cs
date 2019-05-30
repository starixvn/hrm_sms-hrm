namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum OvertimeType
    {
        [StringValue("Normal")]
        Normal = 1,

        [StringValue("Off")]
        Off = 2,

        [StringValue("Holiday")]
        Holiday = 3,
    }
}