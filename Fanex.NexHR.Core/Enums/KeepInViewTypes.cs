namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum KeepInViewTypes
    {
        [StringValue("Keep In View By Day")]
        KeepInViewByDay = 1,

        [StringValue("Keep In View By Hour")]
        KeepInViewByHour = 2
    }
}