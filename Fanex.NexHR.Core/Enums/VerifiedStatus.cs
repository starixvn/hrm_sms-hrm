namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum VerifiedStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Verified")]
        Verified = 2,

        [StringValue("NotVerified")]
        NotVerified = 3
    }
}
