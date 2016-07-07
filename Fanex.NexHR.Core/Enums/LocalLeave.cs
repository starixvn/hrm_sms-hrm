namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum LocalLeave
    {
        [StringValue("None")]
        None = 0,

        [StringValue("Standard leave")]
        StandardLeave = 1,

        [StringValue("Per 5 Years accumulation leave")]
        AccumulationLeave = 2,

        [StringValue("Keep in view leave")]
        KIVLeave = 3
    }
}
