namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum MarriageStatus
    {
        [StringValue("Default")]
        Default = 0,

        [StringValue("Married")]
        Married = 1,

        [StringValue("Divorced")]
        Divorced = 2,

        [StringValue("Single")]
        Single = 3,

        [StringValue("Widow")]
        Widow = 4
    }
}
