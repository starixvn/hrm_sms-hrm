namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum ApprovedStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Approved")]
        Approved = 2,

        [StringValue("Rejected")]
        Rejected = 3
    }
}
