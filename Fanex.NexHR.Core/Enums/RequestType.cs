namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum RequestType
    {
        [StringValue("Check In")]
        CheckIn = 1,

        [StringValue("Check Out")]
        CheckOut = 2,

        [StringValue("Late In")]
        LateIn = 3,

        [StringValue("Early Out")]
        EarlyOut = 4,

        [StringValue("Change Shift")]
        ChangeShift = 5,

        [StringValue("Leave")]
        Leave = 6,

        [StringValue("KeepInView")]
        KeepInView = 7,

        [StringValue("Overtime")]
        Overtime = 8
    }
}