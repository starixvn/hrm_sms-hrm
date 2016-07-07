namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum Command
    {
        [StringValue("Accept")]
        Accept = 1,

        [StringValue("NotAccept")]
        NotAccept = 2,

        [StringValue("Approve")]
        Approve = 3,

        [StringValue("Reject")]
        Reject = 4,

        [StringValue("Process")]
        Process = 5,

        [StringValue("Void")]
        Void = 6,

        [StringValue("Reset")]
        Reset = 7
    }
}
