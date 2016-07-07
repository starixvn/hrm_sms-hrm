namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum HRStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Processed")]
        Processed = 2,

        [StringValue("Void")]
        Void = 3
    }
}
