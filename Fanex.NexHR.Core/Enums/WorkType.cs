namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum WorkType : int
    {
        [StringValue("Off")]
        Off = 1,

        [StringValue("Leave")]
        Leave = 2,

        [StringValue("Work")]
        Work = 3
    }
}
