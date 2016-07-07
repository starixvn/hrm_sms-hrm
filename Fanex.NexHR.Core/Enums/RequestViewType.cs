namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum RequestViewType
    {
        [StringValue("Personal")]
        PersonalSpace = 1,

        [StringValue("HR Space")]
        HRSpace = 2,

        [StringValue("Company Space")]
        CompanySpace = 3,

        [StringValue("SA Space")]
        SASpace = 4
    }
}
