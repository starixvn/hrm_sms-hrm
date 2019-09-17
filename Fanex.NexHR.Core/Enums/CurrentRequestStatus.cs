namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum CurrentRequestStatus
    {
        [StringValue("N/A")]
        NA = 0,

        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Verified")]
        Verified = 2,

        [StringValue("NotVerified")]
        NotVerified = 3,

        [StringValue("Approved")]
        Approved = 4,

        [StringValue("Rejected")]
        Rejected = 5,

        [StringValue("Processed")]
        Processed = 6,

        [StringValue("Void")]
        Void = 7
    }
}