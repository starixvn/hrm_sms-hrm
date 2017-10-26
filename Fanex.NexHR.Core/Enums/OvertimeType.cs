namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
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